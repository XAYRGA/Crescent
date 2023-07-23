plugin = {} 
local environments = {}

-- Loads plugins 

local a = file.Find("moonosc/plugins/","*.lua")
for k,v in pairs(a) do
	local pluginEnv = {
		GUID = nil,
		NAME = nil, 
		AUTHOR = nil,  
	}
	setmetatable(pluginEnv,{__index = _G})
	local func,err = loadfile(v)
	if (func==nil) then 
		local err2 = string.format("Failed to load plugin: %s (Compile failure)\n\n", v,err)
		system.error(err2)
	else 
		setfenv(func, pluginEnv)
		local s,e = pcall(func) 
		if (s) then 
			if (not pluginEnv.GUID) then 
				local err = string.format("Failed to load plugin: %s (no GUID)", v)
				system.error(err)
				event.destroy(pluginEnv)
			else 
				local txt = string.format("Loaded plugin (%s) %s by %s from %s",pluginEnv.GUID,pluginEnv.NAME,pluginEnv.AUTHOR,v);
				print(txt)
				environments[pluginEnv.GUID] = pluginEnv
			end 
		else 
			system.error("Plugin Error: " .. v .."\n" .. e)
		end 
	end 
end 


function plugin.getList() 
	local keys = {}
	for key,value in pairs(environments) do 
		keys[#keys + 1] = key
	end 
	return keys
end 

function plugin.getEnvironment(guid)
	local env = environments[guid]
	if (env) then 
		return env
	end 
end 


