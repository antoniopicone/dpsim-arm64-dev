import socket
import json
from datetime import datetime

def start_receiver(host='localhost', port=12001):
    """Start a UDP receiver to listen for phasor data.
    
    Args:
        host (str): Host address to listen on
        port (int): UDP port number to listen on
    """
    # Create UDP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.bind((host, port))
    print(f"UDP receiver listening on {host}:{port}")
    
    try:
        while True:
            # Receive data
            data, addr = sock.recvfrom(4096)
            timestamp = datetime.now().strftime('%Y-%m-%d %H:%M:%S.%f')
            
            try:
                # Parse JSON data
                json_data = json.loads(data.decode())
                
                # Print received data
                print(f"\n[{timestamp}] Received from {addr}:")
                if 'time' in json_data:
                    print(f"Simulation time: {json_data['time']}")
                    for name, value in json_data['values'].items():
                        print(f"{name}:")
                        print(f"  Magnitude: {value['magnitude']:.4f}")
                        print(f"  Angle: {value['angle']:.2f}°")
                        print(f"  Complex: {value['real']:.4f} + {value['imag']:.4f}j")
                else:
                    print(f"Magnitude: {json_data['magnitude']:.4f}")
                    print(f"Angle: {json_data['angle']:.2f}°")
                    print(f"Complex: {json_data['real']:.4f} + {json_data['imag']:.4f}j")
            
            except json.JSONDecodeError as e:
                print(f"Error decoding JSON: {e}")
            except Exception as e:
                print(f"Error processing data: {e}")
    
    except KeyboardInterrupt:
        print("\nShutting down receiver...")
    finally:
        sock.close()

if __name__ == "__main__":
    start_receiver()
