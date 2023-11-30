-- Video game Protocol Dissector
-- Written by Blake Bryant
-- Date: 16 April 2022
-- Latestvideogame source code available on github via: https://github.com/bbryantku/unity_fps_game_netcode/
------- Original videogame base source code written by Tom Weiland and available at: : https://github.com/tom-weiland/tcp-TCP-networking/
-- Useful References
-------https://www.wireshark.org/docs/wsdg_html_chunked/lua_module_Pinfo.html
-------https://mika-s.github.io/wireshark/lua/dissector/2017/11/04/creating-a-wireshark-dissector-in-lua-1.html
-------
-- Project files that provide insight to packet structure used by the client and server programs
---- GameServer/Assets/Scripts/ServerSend.cs shows the server packet structures
---- GameClient/Assets/Scripts/ClientSend.cs shows client packet structures

fps_game_netcode_TCP_protocol = Proto("fps_game_netcode_TCP",  "fps_game_netcode Protocol TCP")

payload_length             = ProtoField.int32 ("fps_game_netcode_TCP.payload_length"              , "payload_length"              , base.DEC)
player_input_length        = ProtoField.int32 ("fps_game_netcode_TCP.player_input_length"         , "player_input_length"         , base.DEC)
packet_type                = ProtoField.int32 ("fps_game_netcode_TCP.packet_type"                 , "packetType"                  , base.DEC)
packet_num                 = ProtoField.int32 ("fps_game_netcode_TCP.packet_num"                  , "packetNum"                   , base.DEC)
welcome_message            = ProtoField.string("fps_game_netcode_TCP.welcome_message"             , "welcomeMessage"              , base.ASCII)
client_id                  = ProtoField.int32 ("fps_game_netcode_TCP.client_id"                   , "clientID"                    , base.DEC)
player_id                  = ProtoField.int32 ("fps_game_netcode_TCP.player_id"                   , "playerID"                    , base.DEC)
player_username            = ProtoField.string("fps_game_netcode_TCP.player_username"             , "playerUserName"              , base.ASCII)
player_psn_x               = ProtoField.float ("fps_game_netcode_TCP.player_psn_x"                , "player_psn_x"                , base.HEX)
player_psn_y               = ProtoField.float ("fps_game_netcode_TCP.player_psn_y"                , "player_psn_y"                , base.HEX)
player_psn_z               = ProtoField.float ("fps_game_netcode_TCP.player_psn_z"                , "player_psn_z"                , base.HEX)
player_quaternion_x        = ProtoField.float ("fps_game_netcode_TCP.player_quaternion_x"         , "player_quaternion_x"         , base.HEX)
player_quaternion_y        = ProtoField.float ("fps_game_netcode_TCP.player_quaternion_y"         , "player_quaternion_y"         , base.HEX)
player_quaternion_z        = ProtoField.float ("fps_game_netcode_TCP.player_quaternion_z"         , "player_quaternion_z"         , base.HEX)
player_quaternion_w        = ProtoField.float ("fps_game_netcode_TCP.player_quaternionw"          , "player_quaternion_w"         , base.HEX)
player_health              = ProtoField.float ("fps_game_netcode_TCP.player_health"               , "playerHealth"                , base.DEC)
spawner_id                 = ProtoField.int32 ("fps_game_netcode_TCP.spawner_id"                  , "spawnerID"                   , base.DEC)
spawner_psn_x              = ProtoField.float ("fps_game_netcode_TCP.spawner_psn_x"               , "spawner_psn_x"               , base.HEX)
spawner_psn_y              = ProtoField.float ("fps_game_netcode_TCP.spawner_psn_y"               , "spawner_psn_y"               , base.HEX)
spawner_psn_z              = ProtoField.float ("fps_game_netcode_TCP.spawner_psn_z"               , "spawner_psn_z"               , base.HEX)
spawner_has_item           = ProtoField.bool  ("fps_game_netcode_TCP.spawner_has_item"            , "spawnerHasItem"              , base.NONE)
projectile_id              = ProtoField.int32 ("fps_game_netcode_TCP.projectile_id"               , "projectileID"                , base.DEC)
projectile_psn_x           = ProtoField.float ("fps_game_netcode_TCP.projectile_psn_x"            , "projectile_psn_x"            , base.HEX)
projectile_psn_y           = ProtoField.float ("fps_game_netcode_TCP.projectile_psn_y"            , "projectile_psn_y"            , base.HEX)
projectile_psn_z           = ProtoField.float ("fps_game_netcode_TCP.projectile_psn_z"            , "projectile_psn_z"            , base.HEX)
projectile_thrown_by_player= ProtoField.bool  ("fps_game_netcode_TCP.projectile_thrown_by_player" , "projectile_thrown_by_player" , base.NONE)
enemy_id                   = ProtoField.int32 ("fps_game_netcode_TCP.enemy_id"                    , "enemyID"                     , base.DEC)
enemy_psn_x                = ProtoField.float ("fps_game_netcode_TCP.enemy_psn_x"                 , "enemy_psn_x"                 , base.HEX)
enemy_psn_y                = ProtoField.float ("fps_game_netcode_TCP.enemy_psn_y"                 , "enemy_psn_y"                 , base.HEX)
enemy_psn_z                = ProtoField.float ("fps_game_netcode_TCP.enemy_psn_z"                 , "enemy_psn_z"                 , base.HEX)
enemy_health               = ProtoField.float ("fps_game_netcode_TCP.enemy_health"                , "enemyHealth"                 , base.HEX)
player_w_key               = ProtoField.bool  ("fps_game_netcode_TCP.player_w_key"                , "player_w_key"                , base.NONE)
player_s_key               = ProtoField.bool  ("fps_game_netcode_TCP.player_s_key"                , "player_s_key"                , base.NONE)
player_a_key               = ProtoField.bool  ("fps_game_netcode_TCP.player_a_key"                , "player_a_key"                , base.NONE)
player_d_key               = ProtoField.bool  ("fps_game_netcode_TCP.player_d_key"                , "player_d_key"                , base.NONE)
player_space_key           = ProtoField.bool  ("fps_game_netcode_TCP.player_space_key"            , "player_space_key"            , base.NONE)

fps_game_netcode_TCP_protocol.fields = {client_id, payload_length, player_input_length, packet_type, packet_num, welcome_message, player_username, player_id, player_psn_x, 
                            player_psn_y,player_psn_z, player_quaternion_x, player_quaternion_y, player_quaternion_z, 
                            player_quaternion_w, player_health, spawner_id, spawner_psn_x, spawner_psn_y, spawner_psn_z, 
                            spawner_has_item, projectile_id, projectile_psn_x, projectile_psn_y, projectile_psn_z, 
                            projectile_thrown_by_player, enemy_id, enemy_psn_x, enemy_psn_y, enemy_psn_z, enemy_health, 
                            player_w_key, player_s_key, player_a_key, player_d_key, player_space_key}  

game_protocol_number = 26950

function fps_game_netcode_TCP_protocol.dissector(buffer, pinfo, tree)
  length = buffer:len()
  if length == 0 then return end

  pinfo.cols.protocol = fps_game_netcode_TCP_protocol.name

  local subtree = tree:add_le(fps_game_netcode_TCP_protocol, buffer(), "fps_game_netcode Protocol TCP Data")
    subtree:add_le(payload_length, buffer(0,4))

  local packet_type_number = buffer(4,4):le_uint()
  local packet_type_name = "unknown"

  -- Welcome packet sent to Client  via TCP
  if packet_type_number == 1 and pinfo.src_port == game_protocol_number then
    packet_type_name = "Server Welcome"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(welcome_message, buffer(12,22))
    subtree:add_le(client_id, buffer(34,4))
  end

  -- Welcome packet sent to Server via TCP
  if packet_type_number == 1 and pinfo.dst_port == game_protocol_number then
    packet_type_name = "Client Welcome"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(client_id, buffer(8,4))
    local string_length
    for i = 16, length - 1, 1 do
      if (buffer(i,1):le_uint() == 0) then
        string_length = i - 16
        break
      end
    end
  subtree:add_le(player_username, buffer(16,string_length))
  end

  --Spwan Player packet sent to Client via TCP
  if packet_type_number == 2 and pinfo.src_port == game_protocol_number then
    packet_type_name = "spawn Player"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(player_id, buffer(8,4))
    local string_length
    for i = 16, length - 1, 1 do
      if (buffer(i,1):le_uint() == 0) then
        string_length = i - 16
        break
      end
    end
    subtree:add_le(player_username,             buffer(16,string_length))
    subtree:add_le(player_psn_x,                buffer(16+string_length,4))
    subtree:add_le(player_psn_y,                buffer(16+string_length+4,4))
    subtree:add_le(player_psn_z,                buffer(16+string_length+8,4))
    subtree:add_le(player_quaternion_x,         buffer(16+string_length+12,4))
    subtree:add_le(player_quaternion_y,         buffer(16+string_length+16,4))
    subtree:add_le(player_quaternion_z,         buffer(16+string_length+20,4))
    subtree:add_le(player_quaternion_w,         buffer(16+string_length+24,4))
  end

  --Player Shoot sent to Server via TCP
  if packet_type_number == 3 and pinfo.dst_port == game_protocol_number then
    packet_type_name = "player shoot"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(player_psn_x,                buffer(8,4))
    subtree:add_le(player_psn_y,                buffer(12,4))
    subtree:add_le(player_psn_z,                buffer(16,4))
    subtree:add_le(packet_num,                  buffer(20,4))
  end

  --Player Throw sent to Server via TCP
  if packet_type_number == 4 and pinfo.dst_port == game_protocol_number then
    packet_type_name = "player Throw Projectile"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(player_quaternion_x,         buffer(8,4))
    subtree:add_le(player_quaternion_y,         buffer(12,4))
    subtree:add_le(player_quaternion_z,         buffer(16,4))
    subtree:add_le(packet_num,                  buffer(20,4))
  end

  --Player disconnected sent to Client via TCP
  if packet_type_number == 5 then
    packet_type_name = "player disconnected"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(player_id,                   buffer(8,4))
  end

  --Player Health sent to Client via TCP
  if packet_type_number == 6 then
    packet_type_name = "player health"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(player_id,                   buffer(8,4))
    subtree:add_le(player_health,               buffer(12,4))
  end

  --Player Respawned sent to Client via TCP
  if packet_type_number == 7 then
    packet_type_name = "player respawned"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(player_id,                   buffer(8,4))
  end

  --Create Item Spawner sent to Client via TCP
  if packet_type_number == 8 then
    packet_type_name =  "create item spawner"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(spawner_id,                  buffer(8,4))
    subtree:add_le(spawner_psn_x,               buffer(12,4))
    subtree:add_le(spawner_psn_y,               buffer(16,4))
    subtree:add_le(spawner_psn_z,               buffer(20,4))
    subtree:add_le(spawner_has_item,            buffer(24,1))
  end

  --Item Spawned sent to Client via TCP
  if packet_type_number == 9 then
    packet_type_name =  "item spawned"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(spawner_id,                  buffer(8,4))
  end

  --Item picked up sent to Client via TCP
  if packet_type_number == 10 then
    packet_type_name =  "item picked up"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(spawner_id,                  buffer(8,4))
    subtree:add_le(player_id,                   buffer(12,4))
  end

  --Spawn Projectile sent to Client via TCP
  if packet_type_number == 11 then
    packet_type_name =  "spawn projectile"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(projectile_id,               buffer(8,4))
    subtree:add_le(projectile_psn_x,            buffer(12,4))
    subtree:add_le(projectile_psn_y,            buffer(16,4))
    subtree:add_le(projectile_psn_z,            buffer(20,4))
    subtree:add_le(projectile_thrown_by_player, buffer(24,1))
    subtree:add_le(packet_num,                  buffer(25,4))
  end

  --Projectile exploded sent to client via TCP
  if packet_type_number == 13 then
    packet_type_name =  "projectile exploded"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(projectile_id,               buffer(8,4))
    subtree:add_le(projectile_psn_x,            buffer(12,4))
    subtree:add_le(projectile_psn_y,            buffer(16,4))
    subtree:add_le(projectile_psn_z,            buffer(20,4))
  end

  --Spawn enemy sent to client via TCP
  if packet_type_number == 14 then
    packet_type_name =  "spawn enemy"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(enemy_id,                    buffer(8,4))
    subtree:add_le(enemy_psn_x,                 buffer(12,4))
    subtree:add_le(enemy_psn_y,                 buffer(16,4))
    subtree:add_le(enemy_psn_z,                 buffer(20,4))
  end

  --Enemy health sent to client via TCP
  if packet_type_number == 16 then
    packet_type_name =  "enemy health"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

    subtree:add_le(enemy_id,                    buffer(8,4))
    subtree:add_le(enemy_health,                buffer(12,4))
  end

end

local tcp_port = DissectorTable.get("tcp.port")
tcp_port:add(game_protocol_number, fps_game_netcode_TCP_protocol)
