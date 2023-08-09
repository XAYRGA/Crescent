print(
[[   ___                        _   
  / __|_ _ ___ ___ __ ___ _ _| |_ 
 | (__| '_/ -_|_-</ _/ -_) ' \  _|
  \___|_| \___/__/\__\___|_||_\__|
                                  
                                  ]])


print("xayrga 2023 https://github.com/xayrga/crescent")

dofile("crescent/table.lua")
dofile("crescent/math.lua")
dofile("crescent/env.lua")
dofile("crescent/events.lua")
dofile("crescent/avatar.lua")
dofile("crescent/timer.lua")
dofile("crescent/json.lua")

function SYSTEM_Update()
	event.RaiseGlobal("update")
end 

function SYSTEM_IngestOSCData(path, data) 
	event.RaiseGlobal("OSCData",path,data)
	event.RaiseGlobal("osc:" .. path,data)
end

dofile("crescent/plugin.lua")

event.RaiseGlobal("PostInit")


