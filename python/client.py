import socket

if __name__ == '__main__':
    s = socket.socket()
    address = '127.0.0.1'
    port = 50000 # port number is a number, not string
    s.connect((address, port))
    s.sendall("hello there".encode())
    print(s.recv(1024).decode('ascii'))
    s.close()
