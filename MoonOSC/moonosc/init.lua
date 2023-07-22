print("<unnamed project>")
print("xayrga 2023 https://github.com/xayrga/<unspecified project repo>")

dofile("moonosc/math.lua")
dofile("moonosc/env.lua")
dofile("moonosc/events.lua")
dofile("moonosc/avatar.lua")




function SYSTEM_Update()
	event.RaiseGlobal("update")
end 


function SYSTEM_IngestOSCData(path, data) 
	event.RaiseGlobal("OSCData",path,data)
	event.RaiseGlobal("osc:" .. path,path,data)
end



local function getKeys(t) 
	local r = {}
	for k,v in pairs(t) do 
		r[#r + 1] = k
	end 
	return r
end 

function PrintTable( t, indent, done )
	done = done or {}
	indent = indent or 0
	local keys = getKeys( t )

	done[ t ] = true

	for i = 1, #keys do
		local key = keys[ i ]
		local value = t[ key ]
		print( string.rep( "\t", indent ) )

		if  ( type( value ) == "table" and not done[ value ] ) then

			done[ value ] = true
			print( key, ":\n" )
			PrintTable ( value, indent + 2, done )
			done[ value ] = nil
		else
			print( key, "\t=\t", value, "\n" )
		end
	end
end



local a = file.Find("moonosc/plugins/","*.lua")
for k,v in pairs(a) do
	
	local pluginEnv = {
		NAME = "", 
		AUTHOR = "",  
	}
	setmetatable(pluginEnv,{__index = _G})
	local func,err = loadfile(v)
	if (func==nil) then 
		print("LOAD PLUGIN FAIL " .. v .. "\n" .. tostring(err))
	else 
		setfenv(func, pluginEnv)
		local s,e = pcall(func) 
		if (s) then 
			print("Loaded Plugin " .. pluginEnv.NAME .. " by " .. pluginEnv.AUTHOR .. " from " .. v)
		else 
			print("Plugin Error: " .. v .."\n" .. e)
		end 
	end 
end 