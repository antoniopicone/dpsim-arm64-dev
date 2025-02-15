import socket
import json
import numpy as np

class UDPLogger:
    def __init__(self, host='localhost', port=12001):
        """Initialize UDP socket for logging phasor values.
        
        Args:
            host (str): Host address to send data to
            port (int): UDP port number
        """
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.address = (host, port)
    
    def log_phasor(self, phasor_value):
        """Log a phasor value by sending it over UDP.
        
        Args:
            phasor_value: Complex phasor value to log
        """
        # Convert complex number to dict with real and imaginary parts
        data = {
            'real': float(phasor_value.real),
            'imag': float(phasor_value.imag),
            'magnitude': abs(phasor_value),
            'angle': np.angle(phasor_value, deg=True)
        }
        
        # Convert to JSON string and encode to bytes
        json_data = json.dumps(data)
        self.sock.sendto(json_data.encode(), self.address)
    
    def log_attributes(self, time, **attributes):
        """Log multiple attributes with their values at a specific time.
        
        Args:
            time (float): Simulation time
            **attributes: Dictionary of attribute names and their complex values
        """
        data = {
            'time': time,
            'values': {}
        }
        
        for name, value in attributes.items():
            data['values'][name] = {
                'real': float(value.real),
                'imag': float(value.imag),
                'magnitude': abs(value),
                'angle': np.angle(value, deg=True)
            }
        
        # Convert to JSON string and encode to bytes
        json_data = json.dumps(data)
        self.sock.sendto(json_data.encode(), self.address)
    
    def close(self):
        """Close the UDP socket."""
        self.sock.close()

# Example custom logger for DPsim that uses UDP
class DPSimUDPLogger:
    def __init__(self, name, host='localhost', port=12001):
        """Initialize DPsim UDP logger.
        
        Args:
            name (str): Logger name
            host (str): UDP host address
            port (int): UDP port number
        """
        self.name = name
        self.udp_logger = UDPLogger(host, port)
        self.attributes = {}
    
    def log_attribute(self, name, attr_type, component):
        """Register an attribute to be logged.
        
        Args:
            name (str): Attribute name
            attr_type (str): Attribute type
            component: DPsim component to log
        """
        self.attributes[name] = (attr_type, component)
    
    def log(self, time):
        """Log all registered attributes at current simulation time.
        
        Args:
            time (float): Current simulation time
        """
        values = {}
        for name, (attr_type, component) in self.attributes.items():
            # Get the attribute value from the component
            # This assumes the component has a method to get the value
            # You'll need to adapt this based on DPsim's actual API
            value = getattr(component, attr_type, None)
            if value is not None:
                values[name] = value
        
        self.udp_logger.log_attributes(time, **values)
    
    def close(self):
        """Close the UDP logger."""
        self.udp_logger.close()
