# unity_fps_game_netcode
A simple client &amp; server program for video game research and project-based learning 

This project is an extension of a previous project created by Tom Weiland available on github at: https://github.com/tom-weiland/tcp-udp-networking/

Tom's project is also accompanied with a great YouTube step-by-step tutorial explaining how he developed the base code for this project, available here: https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5 

Notable modifications contained within this project include:
- improved logging functionality
- incorporation of player bot AI routine that replicates keyboard and mouse input
- addition of counters to packet structures (to assist in latency calculations)
- LUA based wireshark protocol dissectors to provide for realtime network analysis
