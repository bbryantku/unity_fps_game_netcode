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

fps_game_netcode_protocol = Proto("fps_game_netcode",  "fps_game_netcode Protocol")

packet_length              = ProtoField.int32 ("fps_game_netcode.packet_length"               , "packetLength"                , base.DEC)
packet_type                = ProtoField.int32 ("fps_game_netcode.packet_type"                 , "packetType"                  , base.DEC)
welcome_message            = ProtoField.string("fps_game_netcode.welcome_message"            , "welcomeMessage"               , base.ASCII)
client_id                  = ProtoField.int32 ("fps_game_netcode.client_id"                   , "clientID"                    , base.DEC)
player_id                  = ProtoField.int32 ("fps_game_netcode.player_id"                   , "playerID"                    , base.DEC)
player_username            = ProtoField.string("fps_game_netcode.player_username"            , "playerUserName"               , base.ASCII)
player_psn_x               = ProtoField.float ("fps_game_netcode.player_psn_x"                , "player_psn_x"                , base.DEC)
player_psn_y               = ProtoField.float ("fps_game_netcode.player_psn_y"                , "player_psn_y"                , base.DEC)
player_psn_z               = ProtoField.float ("fps_game_netcode.player_psn_z"                , "player_psn_z"                , base.DEC)
player_quaternion_x        = ProtoField.float ("fps_game_netcode.player_quaternion_x"             , "player_quaternion_x"         , base.DEC)
player_quaternion_y        = ProtoField.float ("fps_game_netcode.player_quaternion_y"         , "player_quaternion_y"         , base.DEC)
player_quaternion_z        = ProtoField.float ("fps_game_netcode.player_quaternion_z"         , "player_quaternion_z"         , base.DEC)
player_quaternion_w = ProtoField.float ("fps_game_netcode.player_quaternionw"  , "player_quaternionw"  , base.DEC)
player_health              = ProtoField.float ("fps_game_netcode.player_health"               , "playerHealth"                , base.DEC)
spawner_id                 = ProtoField.int32 ("fps_game_netcode.spawner_id"                  , "spawnerID"                   , base.DEC)
spawner_psn_x              = ProtoField.float ("fps_game_netcode.spawner_psn_x"               , "spawner_psn_x"               , base.DEC)
spawner_psn_y              = ProtoField.float ("fps_game_netcode.spawner_psn_y"               , "spawner_psn_y"               , base.DEC)
spawner_psn_z              = ProtoField.float ("fps_game_netcode.spawner_psn_z"               , "spawner_psn_z"               , base.DEC)
spawner_has_item           = ProtoField.bool  ("fps_game_netcode.spawner_has_item"            , "spawnerHasItem"              , base.none)
projectile_id              = ProtoField.int32 ("fps_game_netcode.projectile_id"               , "projectileID"                , base.DEC)
projectile_psn_x           = ProtoField.float ("fps_game_netcode.projectile_psn_x"            , "projectile_psn_x"            , base.DEC)
projectile_psn_y           = ProtoField.float ("fps_game_netcode.projectile_psn_y"            , "projectile_psn_y"            , base.DEC)
projectile_psn_z           = ProtoField.float ("fps_game_netcode.projectile_psn_z"            , "projectile_psn_z"            , base.DEC)
projectile_thrown_by_player= ProtoField.bool  ("fps_game_netcode.projectile_thrown_by_player" , "projectile_thrown_by_player" , base.none)
enemy_id                   = ProtoField.int32 ("fps_game_netcode.enemy_id"                    , "enemyID"                     , base.DEC)
enemy_psn_x                = ProtoField.float ("fps_game_netcode.enemy_psn_x"                 , "enemy_psn_x"                 , base.DEC)
enemy_psn_y                = ProtoField.float ("fps_game_netcode.enemy_psn_y"                 , "enemy_psn_y"                 , base.DEC)
enemy_psn_z                = ProtoField.float ("fps_game_netcode.enemy_psn_z"                 , "enemy_psn_z"                 , base.DEC)
enemy_health               = ProtoField.float ("fps_game_netcode.enemy_health"                , "enemyHealth"                 , base.DEC)
player_w_key               = ProtoField.int32 ("fps_game_netcode.player_w_key"                , "player_w_key"                , base.DEC)
player_s_key               = ProtoField.int32 ("fps_game_netcode.player_s_key"                , "player_s_key"                , base.DEC)
player_a_key               = ProtoField.int32 ("fps_game_netcode.player_a_key"                , "player_a_key"                , base.DEC)
player_d_key               = ProtoField.int32 ("fps_game_netcode.player_d_key"                , "player_d_key"                , base.DEC)
player_space_key           = ProtoField.int32 ("fps_game_netcode.player_space_key"            , "player_space_key"            , base.DEC)

fps_game_netcode_protocol.fields = {client_id, packet_length, packet_type, welcome_message, player_username, player_id, player_psn_x, 
                            player_psn_y,player_psn_z, player_quaternion_x, player_quaternion_y, player_quaternion_z, 
                            player_quaternion_w, player_health, spawner_id, spawner_psn_x, spawner_psn_y, spawner_psn_z, 
                            spawner_has_item, projectile_id, projectile_psn_x, projectile_psn_y, projectile_psn_z, 
                            projectile_thrown_by_player, enemy_id, enemy_psn_x, enemy_psn_y, enemy_psn_z, enemy_health, 
                            player_w_key, player_s_key, player_a_key, player_d_key, player_space_key}  

game_protocol_number = 26950

function fps_game_netcode_protocol.dissector(buffer, pinfo, tree)
  length = buffer:len()
  if length == 0 then return end

  pinfo.cols.protocol = fps_game_netcode_protocol.name

  local subtree = tree:add_le(fps_game_netcode_protocol, buffer(), "fps_game_netcode Protocol Data")
    subtree:add_le(packet_length, buffer(0,4))

  local packet_type_number = buffer(4,4):le_uint()
  packet_type_name = get_packet_type(packet_type_number,pinfo)  
  subtree:add_le(packet_type, buffer(4,4)):append_text(" (".. packet_type_name .. ")")

  if packet_type_number == 1 and pinfo.src_port == game_protocol_number then
    subtree:add_le(welcome_message, buffer(12,22))
    subtree:add_le(client_id, buffer(34,4))
  end

  if packet_type_number == 1 and pinfo.dst_port == game_protocol_number then
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

  if packet_type_number == 2 and pinfo.src_port == game_protocol_number then
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
    subtree:add_le(player_quaternion_w,  buffer(16+string_length+24,4))
  end

  if packet_type_number == 2 and pinfo.dst_port == game_protocol_number then
    subtree:add_le(player_w_key,                buffer(8,4))
    subtree:add_le(player_s_key,                buffer(12,4))
    subtree:add_le(player_a_key,                buffer(16,4))
    subtree:add_le(player_d_key,                buffer(20,4))
    subtree:add_le(player_space_key,            buffer(24,4))
  end

  if packet_type_number == 3 and pinfo.src_port == game_protocol_number then
    subtree:add_le(player_id,                   buffer(8,4))
    subtree:add_le(player_psn_x,                buffer(12,4))
    subtree:add_le(player_psn_y,                buffer(16,4))
    subtree:add_le(player_psn_z,                buffer(20,4))
  end

  if packet_type_number == 3 and pinfo.dst_port == game_protocol_number then
    subtree:add_le(player_psn_x,                buffer(8,4))
    subtree:add_le(player_psn_y,                buffer(12,4))
    subtree:add_le(player_psn_z,                buffer(16,4))
  end


  if packet_type_number == 4 and pinfo.src_port == game_protocol_number then
    subtree:add_le(player_id,                   buffer(8,4))
    subtree:add_le(player_quaternion_x,         buffer(12,4))
    subtree:add_le(player_quaternion_y,         buffer(16,4))
    subtree:add_le(player_quaternion_z,         buffer(20,4))
    subtree:add_le(player_quaternion_w,  buffer(24,4))
  end

  if packet_type_number == 4 and pinfo.dst_port == game_protocol_number then
    subtree:add_le(player_quaternion_x,         buffer(8,4))
    subtree:add_le(player_quaternion_y,         buffer(12,4))
    subtree:add_le(player_quaternion_z,         buffer(16,4))
  end

  if packet_type_number == 5 then
    subtree:add_le(player_id,                   buffer(8,4))
  end

  if packet_type_number == 6 then
    subtree:add_le(player_id,                   buffer(8,4))
    subtree:add_le(player_health,               buffer(12,4))
  end

  if packet_type_number == 7 then
    subtree:add_le(player_id,                   buffer(8,4))
  end

  if packet_type_number == 8 then
    subtree:add_le(spawner_id,                  buffer(8,4))
    subtree:add_le(spawner_psn_x,               buffer(12,4))
    subtree:add_le(spawner_psn_y,               buffer(16,4))
    subtree:add_le(spawner_psn_z,               buffer(20,4))
    subtree:add_le(spawner_has_item,            buffer(24,4))
  end

  if packet_type_number == 9 then
    subtree:add_le(spawner_id,                  buffer(8,4))
  end

  if packet_type_number == 10 then
    subtree:add_le(spawner_id,                  buffer(8,4))
    subtree:add_le(player_id,                   buffer(12,4))
  end

  if packet_type_number == 11 then
    subtree:add_le(projectile_id,               buffer(8,4))
    subtree:add_le(projectile_psn_x,            buffer(12,4))
    subtree:add_le(projectile_psn_y,            buffer(16,4))
    subtree:add_le(projectile_psn_z,            buffer(20,4))
    subtree:add_le(projectile_thrown_by_player, buffer(24,4))
  end

  if packet_type_number == 12 then
    subtree:add_le(projectile_id,               buffer(8,4))
    subtree:add_le(projectile_psn_x,            buffer(12,4))
    subtree:add_le(projectile_psn_y,            buffer(16,4))
    subtree:add_le(projectile_psn_z,            buffer(20,4))
  end

  if packet_type_number == 13 then
    subtree:add_le(projectile_id,               buffer(8,4))
    subtree:add_le(projectile_psn_x,            buffer(12,4))
    subtree:add_le(projectile_psn_y,            buffer(16,4))
    subtree:add_le(projectile_psn_z,            buffer(20,4))
  end

  if packet_type_number == 14 then
    subtree:add_le(enemy_id,                    buffer(8,4))
    subtree:add_le(enemy_psn_x,                 buffer(12,4))
    subtree:add_le(enemy_psn_y,                 buffer(16,4))
    subtree:add_le(enemy_psn_z,                 buffer(20,4))
  end

  if packet_type_number == 15 then
    subtree:add_le(enemy_id,                    buffer(8,4))
    subtree:add_le(enemy_psn_x,                 buffer(12,4))
    subtree:add_le(enemy_psn_y,                 buffer(16,4))
    subtree:add_le(enemy_psn_z,                 buffer(20,4))
  end

  if packet_type_number == 16 then
    subtree:add_le(enemy_id,                    buffer(8,4))
    subtree:add_le(enemy_health,                buffer(12,4))
  end

end

function get_packet_type(packet_type_number, pinfo)
    local packet_type_description = "unknown"
    local source_port= pinfo.src_port
    local destination_port = pinfo.dst_port
    if source_port              == game_protocol_number then
      if packet_type_number     == 1  then packet_type_description  = "Server Welcome"
      elseif packet_type_number == 2  then packet_type_description  = "spawn Player"
      elseif packet_type_number == 3  then packet_type_description  = "player position"
      elseif packet_type_number == 4  then packet_type_description  = "player rotation"
      elseif packet_type_number == 5  then packet_type_description  = "player disconnected"
      elseif packet_type_number == 6  then packet_type_description  = "player health"
      elseif packet_type_number == 7  then packet_type_description  = "player respawned"
      elseif packet_type_number == 8  then packet_type_description  = "create item spawner"
      elseif packet_type_number == 9  then packet_type_description  = "item spawned"
      elseif packet_type_number == 10 then packet_type_description  = "item picked up"
      elseif packet_type_number == 11 then packet_type_description  = "spawn projectile"
      elseif packet_type_number == 12 then packet_type_description  = "projectile position"
      elseif packet_type_number == 13 then packet_type_description  = "projectile exploded"
      elseif packet_type_number == 14 then packet_type_description  = "spawn enemy"
      elseif packet_type_number == 15 then packet_type_description  = "ememy position"
      elseif packet_type_number == 16 then packet_type_description  = "enemy health"
      end
    end

  if destination_port         == game_protocol_number then
    if packet_type_number     == 1 then packet_type_description     = "Client Welcome"
    elseif packet_type_number == 2 then packet_type_description     = "player movement"  
    elseif packet_type_number == 3 then packet_type_description     = "player shoot"
    elseif packet_type_number == 4 then packet_type_description     = "player throw item"  
    end
  end

    return packet_type_description

end


local tcp_port = DissectorTable.get("tcp.port")
tcp_port:add(game_protocol_number, fps_game_netcode_protocol)
local udp_port = DissectorTable.get("udp.port")
udp_port:add(game_protocol_number, fps_game_netcode_protocol)
