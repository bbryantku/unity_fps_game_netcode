# unity_fps_game_netcode
A simple client &amp; server program for video game research and project-based learning 

This project is an extension of a previous project created by Tom Weiland available on github at: https://github.com/tom-weiland/tcp-udp-networking/

Tom's project is also accompanied with a great YouTube step-by-step tutorial explaining how he developed the base code for this project, available here: https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5 

Notable modifications contained within this project include:
- improved logging functionality
- improved error handling
- incorporation of player bot AI routine that replicates keyboard and mouse input
- ability to run headless gameclient (to assist in network date generation without human interaction)
- ability to adjust client settings via "clientSettings.json" file stored in same directory as game client executable
- ability to adjuct server settings via "serverSettings.json" file stored in same directory as game server executable
- addition of counters to packet structures (to assist in latency calculations)
- LUA based wireshark protocol dissectors to provide for realtime network analysis
  
