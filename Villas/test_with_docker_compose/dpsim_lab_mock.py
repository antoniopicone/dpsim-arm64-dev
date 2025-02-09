import socket
import json
import time
import threading
from queue import Queue
import os

# Lettura delle variabili d'ambiente con valori di default
HOST_DEST = os.getenv('HOST_DEST', 'villas_lab_a')
HOST_SOURCE = os.getenv('HOST_SOURCE', '0.0.0.0')
PORT_DEST = int(os.getenv('PORT_DEST', '12001'))
PORT_SOURCE = int(os.getenv('PORT_SOURCE', '12000'))
SAMPLE_RATE_MILLIS = int(os.getenv('SAMPLE_RATE_MILLIS', '1000'))

def send_udp(udp_socket, server_address, message_queue):
    sequence_number = 11713
    send_data = [{"real": 2.94, "imag": -1.9}]
    
    while True:
        try:
            message = json.dumps([{"sequence": sequence_number, "data": send_data}]).encode('utf-8')
            udp_socket.sendto(message, server_address)
            print(f"Sent to {HOST_DEST}: {message}")
            
            message_queue.put({
                "sequence": sequence_number,
                "timestamp": time.time()
            })
            
            sequence_number += 1
            time.sleep(SAMPLE_RATE_MILLIS/1000)
            
        except Exception as e:
            print(f"Error in send thread: {e}")
            break

def receive_udp(udp_socket, message_queue):
    while True:
        try:
            received_bytes, _ = udp_socket.recvfrom(4096)
            received_message = json.loads(received_bytes.decode('utf-8'))
            print(f"Received from {HOST_DEST}: {received_message}")
            
            if not message_queue.empty():
                sent_info = message_queue.get()
                latency = time.time() - sent_info["timestamp"]
                print(f"Message latency: {latency:.3f} seconds")
                
        except Exception as e:
            print(f"Error in receive thread: {e}")
            break

def main():
    udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    print(f"Connecting to {HOST_DEST}:{PORT_DEST}")
    print(f"Listening on {HOST_SOURCE}:{PORT_SOURCE}")
    server_address = (HOST_DEST, PORT_DEST)
    receive_address = (HOST_SOURCE, PORT_SOURCE)
    udp_socket.bind(receive_address)
    
    message_queue = Queue()
    
    send_thread = threading.Thread(target=send_udp, 
                                 args=(udp_socket, server_address, message_queue))
    receive_thread = threading.Thread(target=receive_udp, 
                                    args=(udp_socket, message_queue))
    
    send_thread.daemon = True
    receive_thread.daemon = True
    
    send_thread.start()
    receive_thread.start()
    
    try:
        while True:
            time.sleep(SAMPLE_RATE_MILLIS/1000)
    except KeyboardInterrupt:
        print("Shutting down...")
        udp_socket.close()

if __name__ == "__main__":
    main()