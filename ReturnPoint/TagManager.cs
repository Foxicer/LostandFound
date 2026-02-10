using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReturnPoint
{
    public class TagCategory
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; } // Hex color code

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        public TagCategory() { }

        public TagCategory(string name, string color)
        {
            Name = name;
            Color = color;
        }
    }

    public class ImageTags
    {
        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; }

        [JsonPropertyName("tags")]
        public Dictionary<string, List<string>> Tags { get; set; } = new Dictionary<string, List<string>>();

        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; } = DateTime.Now;
    }

    public static class TagManager
    {
        private static string TagsStoragePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tags_data.json");
        private static string ImageTagsDir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CapturedImages", "_tags_cache");

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        static TagManager()
        {
            Directory.CreateDirectory(ImageTagsDir);
        }

        /// <summary>
        /// Get all tag categories
        /// </summary>
        public static List<TagCategory> GetAllCategories()
        {
            try
            {
                if (!File.Exists(TagsStoragePath))
                    return GetDefaultCategories();

                string json = File.ReadAllText(TagsStoragePath);
                var categories = JsonSerializer.Deserialize<List<TagCategory>>(json, JsonOptions) ?? new List<TagCategory>();
                return categories.Count == 0 ? GetDefaultCategories() : categories;
            }
            catch
            {
                return GetDefaultCategories();
            }
        }

        /// <summary>
        /// Save all tag categories
        /// </summary>
        public static void SaveCategories(List<TagCategory> categories)
        {
            try
            {
                string json = JsonSerializer.Serialize(categories, JsonOptions);
                File.WriteAllText(TagsStoragePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TagManager] Error saving categories: {ex.Message}");
            }
        }

        /// <summary>
        /// Get default tag categories
        /// </summary>
        private static List<TagCategory> GetDefaultCategories()
        {
            return new List<TagCategory>
            {
                new TagCategory("Color", "#FF6B6B"),
                new TagCategory("Size", "#4ECDC4"),
                new TagCategory("Type", "#45B7D1"),
                new TagCategory("Condition", "#FFA07A"),
                new TagCategory("Location", "#98D8C8")
            };
        }

        /// <summary>
        /// Add a new category
        /// </summary>
        public static bool AddCategory(string name, string color = "#808080")
        {
            var categories = GetAllCategories();
            if (categories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return false;

            categories.Add(new TagCategory(name, color));
            SaveCategories(categories);
            return true;
        }

        /// <summary>
        /// Remove a category
        /// </summary>
        public static bool RemoveCategory(string categoryName)
        {
            var categories = GetAllCategories();
            var category = categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
                return false;

            categories.Remove(category);
            SaveCategories(categories);
            return true;
        }

        /// <summary>
        /// Add a tag to a category
        /// </summary>
        public static bool AddTagToCategory(string categoryName, string tagName)
        {
            var categories = GetAllCategories();
            var category = categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
                return false;

            if (!category.Tags.Contains(tagName))
            {
                category.Tags.Add(tagName);
                category.Tags.Sort();
                SaveCategories(categories);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove a tag from a category
        /// </summary>
        public static bool RemoveTagFromCategory(string categoryName, string tagName)
        {
            var categories = GetAllCategories();
            var category = categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
                return false;

            if (category.Tags.Remove(tagName))
            {
                SaveCategories(categories);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get all tags (flattened across all categories)
        /// </summary>
        public static List<string> GetAllTags()
        {
            var categories = GetAllCategories();
            return categories.SelectMany(c => c.Tags).Distinct().OrderBy(t => t).ToList();
        }

        /// <summary>
        /// Get tags assigned to an image
        /// </summary>
        public static Dictionary<string, List<string>> GetImageTags(string imagePath)
        {
            try
            {
                string cacheFile = GetImageTagsCachePath(imagePath);
                
                // Try to load from cache first
                if (File.Exists(cacheFile))
                {
                    string json = File.ReadAllText(cacheFile);
                    var imageTags = JsonSerializer.Deserialize<ImageTags>(json, JsonOptions);
                    if (imageTags != null)
                        return imageTags.Tags;
                }

                // Try to load from legacy format (plain text files)
                string legacyTagsPath = Path.Combine(Path.GetDirectoryName(imagePath),
                    Path.GetFileNameWithoutExtension(imagePath) + "_tags.txt");

                if (File.Exists(legacyTagsPath))
                {
                    var tags = File.ReadAllLines(legacyTagsPath)
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToList();

                    // Migrate to new format
                    var categorizedTags = new Dictionary<string, List<string>>();
                    categorizedTags["Uncategorized"] = tags;
                    SaveImageTags(imagePath, categorizedTags);
                    return categorizedTags;
                }

                return new Dictionary<string, List<string>>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TagManager] Error getting tags for {imagePath}: {ex.Message}");
                return new Dictionary<string, List<string>>();
            }
        }

        /// <summary>
        /// Save tags for an image
        /// </summary>
        public static void SaveImageTags(string imagePath, Dictionary<string, List<string>> tags)
        {
            try
            {
                string cacheFile = GetImageTagsCachePath(imagePath);
                var imageTags = new ImageTags
                {
                    ImagePath = imagePath,
                    Tags = tags,
                    LastModified = DateTime.Now
                };

                string json = JsonSerializer.Serialize(imageTags, JsonOptions);
                File.WriteAllText(cacheFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TagManager] Error saving tags for {imagePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Add a tag to an image
        /// </summary>
        public static bool AddTagToImage(string imagePath, string categoryName, string tagName)
        {
            try
            {
                var tags = GetImageTags(imagePath);
                
                if (!tags.ContainsKey(categoryName))
                    tags[categoryName] = new List<string>();

                if (!tags[categoryName].Contains(tagName))
                {
                    tags[categoryName].Add(tagName);
                    tags[categoryName].Sort();
                    SaveImageTags(imagePath, tags);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TagManager] Error adding tag: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Remove a tag from an image
        /// </summary>
        public static bool RemoveTagFromImage(string imagePath, string categoryName, string tagName)
        {
            try
            {
                var tags = GetImageTags(imagePath);
                
                if (tags.ContainsKey(categoryName) && tags[categoryName].Remove(tagName))
                {
                    if (tags[categoryName].Count == 0)
                        tags.Remove(categoryName);

                    SaveImageTags(imagePath, tags);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TagManager] Error removing tag: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get the first two tags as a display name
        /// </summary>
        public static string GetDisplayName(string imagePath)
        {
            var tags = GetImageTags(imagePath);
            var allTags = tags.Values.SelectMany(t => t).Take(2).ToList();
            return allTags.Count > 0 ? string.Join(", ", allTags) : Path.GetFileNameWithoutExtension(imagePath);
        }

        /// <summary>
        /// Get the color for a category
        /// </summary>
        public static string GetCategoryColor(string categoryName)
        {
            var categories = GetAllCategories();
            var category = categories.FirstOrDefault(c => c.Name == categoryName);
            return category?.Color ?? "#808080";
        }

        /// <summary>
        /// Get cache file path for image tags
        /// </summary>
        private static string GetImageTagsCachePath(string imagePath)
        {
            string filename = Path.GetFileNameWithoutExtension(imagePath) + ".json";
            return Path.Combine(ImageTagsDir, filename);
        }

        /// <summary>
        /// Delete tags for a deleted image
        /// </summary>
        public static void DeleteImageTags(string imagePath)
        {
            try
            {
                string cacheFile = GetImageTagsCachePath(imagePath);
                if (File.Exists(cacheFile))
                    File.Delete(cacheFile);

                // Also try to delete legacy format
                string legacyTagsPath = Path.Combine(Path.GetDirectoryName(imagePath),
                    Path.GetFileNameWithoutExtension(imagePath) + "_tags.txt");
                if (File.Exists(legacyTagsPath))
                    File.Delete(legacyTagsPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TagManager] Error deleting tags for {imagePath}: {ex.Message}");
            }
        }
    }
}
