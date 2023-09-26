-- Video game Protocol Dissector
-- Written by Blake Bryant
-- Date: 16 April 2022
-- Latestvideogame source code available on github via: https://github.com/bbryantku/unity_fps_game_netcode/
------- Original videogame base source code written by Tom Weiland and available at: : https://github.com/tom-weiland/tcp-udp-networking/
-- Useful References
-------https://www.wireshark.org/docs/wsdg_html_chunked/lua_module_Pinfo.html
-------https://mika-s.github.io/wireshark/lua/dissector/2017/11/04/creating-a-wireshark-dissector-in-lua-1.html
-------
-- Project files that provide insight to packet structure used by the client and server programs
---- GameServer/Assets/Scripts/ServerSend.cs shows the server packet structures
---- GameClient/Assets/Scripts/ClientSend.cs shows client packet structures

fps_game_netcode_UDP_protocol = Proto("fps_game_netcode_UDP",  "fps_game_netcode Protocol UDP")

payload_length             = ProtoField.int32 ("fps_game_netcode_UDP.payload_length"              , "payload_length"              , base.DEC)
player_input_length        = ProtoField.int32 ("fps_game_netcode_UDP.player_input_length"         , "player_input_length"         , base.DEC)
packet_type                = ProtoField.int32 ("fps_game_netcode_UDP.packet_type"                 , "packetType"                  , base.DEC)
packet_num                 = ProtoField.int32 ("fps_game_netcode_UDP.packet_num"                  , "packetNum"                   , base.DEC)
welcome_message            = ProtoField.string("fps_game_netcode_UDP.welcome_message"             , "welcomeMessage"              , base.ASCII)
client_id                  = ProtoField.int32 ("fps_game_netcode_UDP.client_id"                   , "clientID"                    , base.DEC)
player_id                  = ProtoField.int32 ("fps_game_netcode_UDP.player_id"                   , "playerID"                    , base.DEC)
player_username            = ProtoField.string("fps_game_netcode_UDP.player_username"             , "playerUserName"              , base.ASCII)
player_psn_x               = ProtoField.float ("fps_game_netcode_UDP.player_psn_x"                , "player_psn_x"                , base.HEX)
player_psn_y               = ProtoField.float ("fps_game_netcode_UDP.player_psn_y"                , "player_psn_y"                , base.HEX)
player_psn_z               = ProtoField.float ("fps_game_netcode_UDP.player_psn_z"                , "player_psn_z"                , base.HEX)
player_quaternion_x        = ProtoField.float ("fps_game_netcode_UDP.player_quaternion_x"         , "player_quaternion_x"         , base.HEX)
player_quaternion_y        = ProtoField.float ("fps_game_netcode_UDP.player_quaternion_y"         , "player_quaternion_y"         , base.HEX)
player_quaternion_z        = ProtoField.float ("fps_game_netcode_UDP.player_quaternion_z"         , "player_quaternion_z"         , base.HEX)
player_quaternion_w        = ProtoField.float ("fps_game_netcode_UDP.player_quaternionw"          , "player_quaternion_w"         , base.HEX)
player_health              = ProtoField.float ("fps_game_netcode_UDP.player_health"               , "playerHealth"                , base.DEC)
spawner_id                 = ProtoField.int32 ("fps_game_netcode_UDP.spawner_id"                  , "spawnerID"                   , base.DEC)
spawner_psn_x              = ProtoField.float ("fps_game_netcode_UDP.spawner_psn_x"               , "spawner_psn_x"               , base.HEX)
spawner_psn_y              = ProtoField.float ("fps_game_netcode_UDP.spawner_psn_y"               , "spawner_psn_y"               , base.HEX)
spawner_psn_z              = ProtoField.float ("fps_game_netcode_UDP.spawner_psn_z"               , "spawner_psn_z"               , base.HEX)
spawner_has_item           = ProtoField.bool  ("fps_game_netcode_UDP.spawner_has_item"            , "spawnerHasItem"              , base.NONE)
projectile_id              = ProtoField.int32 ("fps_game_netcode_UDP.projectile_id"               , "projectileID"                , base.DEC)
projectile_psn_x           = ProtoField.float ("fps_game_netcode_UDP.projectile_psn_x"            , "projectile_psn_x"            , base.HEX)
projectile_psn_y           = ProtoField.float ("fps_game_netcode_UDP.projectile_psn_y"            , "projectile_psn_y"            , base.HEX)
projectile_psn_z           = ProtoField.float ("fps_game_netcode_UDP.projectile_psn_z"            , "projectile_psn_z"            , base.HEX)
projectile_thrown_by_player= ProtoField.bool  ("fps_game_netcode_UDP.projectile_thrown_by_player" , "projectile_thrown_by_player" , base.NONE)
enemy_id                   = ProtoField.int32 ("fps_game_netcode_UDP.enemy_id"                    , "enemyID"                     , base.DEC)
enemy_psn_x                = ProtoField.float ("fps_game_netcode_UDP.enemy_psn_x"                 , "enemy_psn_x"                 , base.HEX)
enemy_psn_y                = ProtoField.float ("fps_game_netcode_UDP.enemy_psn_y"                 , "enemy_psn_y"                 , base.HEX)
enemy_psn_z                = ProtoField.float ("fps_game_netcode_UDP.enemy_psn_z"                 , "enemy_psn_z"                 , base.HEX)
enemy_health               = ProtoField.float ("fps_game_netcode_UDP.enemy_health"                , "enemyHealth"                 , base.HEX)
player_w_key               = ProtoField.bool  ("fps_game_netcode_UDP.player_w_key"                , "player_w_key"                , base.NONE)
player_s_key               = ProtoField.bool  ("fps_game_netcode_UDP.player_s_key"                , "player_s_key"                , base.NONE)
player_a_key               = ProtoField.bool  ("fps_game_netcode_UDP.player_a_key"                , "player_a_key"                , base.NONE)
player_d_key               = ProtoField.bool  ("fps_game_netcode_UDP.player_d_key"                , "player_d_key"                , base.NONE)
player_space_key           = ProtoField.bool  ("fps_game_netcode_UDP.player_space_key"            , "player_space_key"            , base.NONE)

fps_game_netcode_UDP_protocol.fields = {client_id, payload_length, player_input_length, packet_type, packet_num, welcome_message, player_username, player_id, player_psn_x, 
                            player_psn_y,player_psn_z, player_quaternion_x, player_quaternion_y, player_quaternion_z, 
                            player_quaternion_w, player_health, spawner_id, spawner_psn_x, spawner_psn_y, spawner_psn_z, 
                            spawner_has_item, projectile_id, projectile_psn_x, projectile_psn_y, projectile_psn_z, 
                            projectile_thrown_by_player, enemy_id, enemy_psn_x, enemy_psn_y, enemy_psn_z, enemy_health, 
                            player_w_key, player_s_key, player_a_key, player_d_key, player_space_key}  

game_protocol_number = 26950

function fps_game_netcode_UDP_protocol.dissector(buffer, pinfo, tree)
  length = buffer:len()
  if length == 0 then return end

  pinfo.cols.protocol = fps_game_netcode_UDP_protocol.name
  local packet_type_name = "unknown"
  local packet_type_number = 0
  local subtree = tree:add_le(fps_game_netcode_UDP_protocol, buffer(), "fps_game_netcode Protocol UDP Data")
  local packet_payload_length = 0 -- initialize packet payload length. This has different calcs based on direction
  --Logic for UDP from Server to Client(s)
  if pinfo.src_port == game_protocol_number then
    packet_payload_length = buffer(0,4):le_uint()
    subtree:add_le(payload_length, buffer(0,4))
    packet_type_number = buffer(4,4):le_uint()
  end

  --Logic for UDP from Client to Server
  --Packets sent from client are prefaced with client id first, not payload_length
  --This was horribly confusing. Reference Client.cs Line 400. This should probably be fixed eventually
  if pinfo.dst_port == game_protocol_number then
    subtree:add_le(client_id, buffer(0,4))
    if length < 13 then return end -- occasionally packet only contains player id. This will be a 12 byte packet in total

    packet_payload_length = buffer(4,4):le_uint()
    subtree:add_le(payload_length, buffer(4,4))
    packet_type_number = buffer(8,4):le_uint()
  end

  --Movement packet sent to Server
  --Note Client.UDP.SendData prepends the player ID at beginning of packet
  --This is not done in TCP sent messages, as TCP may be tracked by connection
  if packet_type_number == 2 and pinfo.dst_port == game_protocol_number then
    packet_type_name     = "player movement" 
    subtree:add_le(packet_type, buffer(8,4)):append_text(" (".. packet_type_name .. ")")
    
    local loop_control =0 -- initialize loop control
    local input_length =0 -- initializes input_length as numerical value
    input_length = buffer(12,4):le_int() -- Note this is le_int NOT le_unit.. this allows for arithmetic later
    loop_control = input_length --Set loop to iterate based on input_length
    subtree:add_le(player_input_length, buffer(12,4))

    -- process bool values, each is a single byte
    subtree:add_le(player_w_key,                buffer(16,1))
    subtree:add_le(player_s_key,                buffer(17,1))
    subtree:add_le(player_a_key,                buffer(18,1))
    subtree:add_le(player_d_key,                buffer(19,1))
    subtree:add_le(player_space_key,            buffer(20,1))

    --Process rotation values, each is a 4 byte float
    subtree:add_le(player_quaternion_x,         buffer(21,4))
    subtree:add_le(player_quaternion_y,         buffer(25,4))
    subtree:add_le(player_quaternion_z,         buffer(29,4))
    subtree:add_le(player_quaternion_w,         buffer(33,4))
    subtree:add_le(packet_num,                  buffer(37,4))
  end

  --Player Position sent to Client
  if packet_type_number == 3 and pinfo.src_port == game_protocol_number then
    packet_type_name     = "player position"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")
    subtree:add_le(player_id,                   buffer(8,4))
    subtree:add_le(player_psn_x,                buffer(12,4))
    subtree:add_le(player_psn_y,                buffer(16,4))
    subtree:add_le(player_psn_z,                buffer(20,4))
    subtree:add_le(packet_num,                  buffer(24,4))
  end

  --Player Rotation sent to Client
  if packet_type_number == 4 and pinfo.src_port == game_protocol_number then
    packet_type_name     = "player rotation"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")
    subtree:add_le(player_id,                   buffer(8,4))
    subtree:add_le(player_quaternion_x,         buffer(12,4))
    subtree:add_le(player_quaternion_y,         buffer(16,4))
    subtree:add_le(player_quaternion_z,         buffer(20,4))
    subtree:add_le(player_quaternion_w,         buffer(24,4))
    subtree:add_le(packet_num,                  buffer(28,4))
  end

  --Projectile Position sent to Client
  if packet_type_number == 12 then
    packet_type_name     = "projectile position"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")
    subtree:add_le(projectile_id,               buffer(8,4))
    subtree:add_le(projectile_psn_x,            buffer(12,4))
    subtree:add_le(projectile_psn_y,            buffer(16,4))
    subtree:add_le(projectile_psn_z,            buffer(20,4))
  end

  --Enemy Position sent to Client
  if packet_type_number == 15 then
    packet_type_name     = "ememy position"
    subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")
    subtree:add_le(enemy_id,                    buffer(8,4))
    subtree:add_le(enemy_psn_x,                 buffer(12,4))
    subtree:add_le(enemy_psn_y,                 buffer(16,4))
    subtree:add_le(enemy_psn_z,                 buffer(20,4))
  end

end

local udp_port = DissectorTable.get("udp.port")
udp_port:add(game_protocol_number, fps_game_netcode_UDP_protocol)
