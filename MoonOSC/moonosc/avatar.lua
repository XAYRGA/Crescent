local cached_avatar_data = {}

local searchPath = "/avatar/parameters/"
local searchPathSize = #searchPath + 1
local function ingestData(path, data)
	if (#path < searchPathSize)  then 
		return
	end 
	if (string.sub(path,1,searchPathSize-1)~=searchPath) then 
		return
	end
	local key = string.sub(path,searchPathSize)	
	--print(key)
	cached_avatar_data[key] = data[1]
end 
event.subscribe("OSCData","avatar_ingest", ingestData)


function avatar.getFloat(param) 
	local result = cached_avatar_data[param] 
	return result or 0
end 

function avatar.getInt(param) 
	local result = cached_avatar_data[param] 
	return result or 0
end 

function avatar.getBool(param) 
	local result = cached_avatar_data[param] 
	return result or false
end 