plugin = {} 
local environments = {}
local plugin_sandbox_env = {}

local plugin_sandbox_imports = {
	"package",
	"setmetatable",
	"plugin",
	"vrc",
	"warn",
	--"debug", -- yeaaah no.
	"timer",
	"next",
	"setfenv",
	"dofile",
	"event",
	"pairs",
	"collectgarbage",
	"assert",
	"utf8",
	"Vector",
	"xpcall",
	"select",
	"ovr",
	"error",
	"avatar",
	"tostring",
	"controller",
	"load",
	"_VERSION",
	"getfenv",
	"rawlen",
	"table",
	--"rawequal",
	"http",
	"print",
	"pcall",
	"type",
	--"rawget",
	"webhook",
	"system",
	"math",
	"tonumber",
	"loadfile",
	"require",
	"string",
	"coroutine",
	"ipairs",
	"getmetatable",
	"json"
}




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


for k,v in pairs(plugin_sandbox_imports) do 
	local ref = _G[v]
	if type(ref)=="table" then -- deep copy the library
		plugin_sandbox_env[v] = table.copy(ref)
	else 
		plugin_sandbox_env[v] = ref
	end 
end 


local a = file.Find("crescent/plugins/","*.lua")
for k,v in pairs(a) do
	local pluginEnv = {
		GUID = nil,
		NAME = nil, 
		AUTHOR = nil,  
	}
	setmetatable(pluginEnv,{__index = plugin_sandbox_env})
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

