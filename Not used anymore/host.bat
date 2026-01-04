    import ngrok
    import time

    # Replace 'YOUR_AUTHTOKEN' with your actual ngrok authtoken
    # You can get this from your ngrok dashboard after creating an account
    ngrok.set_auth_token("23yhBixFhvWT3Xx2zQL7CrpVr4T_39CCUfaRAN38CeMu7AqWR")

    # Connect to a local port (e.g., 8000 for a web server)
    listener = ngrok.connect(8000)
    print(f"Ingress established at {listener.url()}")

    # Keep the tunnel alive (e.g., by running your Python app in the background)
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("Disconnecting ngrok tunnel...")
        ngrok.disconnect(listener.url())