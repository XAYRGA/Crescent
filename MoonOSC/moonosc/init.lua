print(
[[  __  __                          
 |  \/  |___  ___ _ _  ___ ___ __ 
 | |\/| / _ \/ _ \ ' \/ _ (_-</ _|
 |_|  |_\___/\___/_||_\___/__/\__|
                                  ]])


print("xayrga 2023 https://github.com/xayrga/moonosc")

dofile("moonosc/math.lua")
dofile("moonosc/env.lua")
dofile("moonosc/events.lua")
dofile("moonosc/avatar.lua")
dofile("moonosc/timer.lua")

function SYSTEM_Update()
	event.RaiseGlobal("update")
end 

function SYSTEM_IngestOSCData(path, data) 
	event.RaiseGlobal("OSCData",path,data)
	event.RaiseGlobal("osc:" .. path,data)
end

dofile("moonosc/plugin.lua")


event.RaiseGlobal("PostInit")