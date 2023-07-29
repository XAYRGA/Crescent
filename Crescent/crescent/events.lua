local subscriptions = {}
local pairs = pairs 
local getfenv = getfenv 
local pcall = pcall 

event = {}

-- Raises an even across all scopes 
function event.RaiseGlobal(name , ... ) 
	for environment,events in pairs(subscriptions) do 
		if events[name] ~= nil then 	
			local evtTable = events[name]
			for uid, func in pairs(evtTable) do 			
				if (func~=nil) then 		
					local status, err = pcall(func, ... )
					if (status==false) then 
						local err_text = string.format("event [%s]-> %s: %s ",name,tostring(uid), tostring(err))
						system.error(err_text)
						break
					end 
				end 			
			end 
		end 
	end 
end 

function event.destroy(realm)
	if (subscriptions[realm]) then 
		subscriptions[realm] = nil
	end 
end 

-- Raises event in local environment
function event.raise(name, ...)
	local env = getfenv(func) 
	local events = subscriptions[env] 
	if events[name] ~= nil then 	
		local evtTable = events[name]
		for uid, func in pairs(evtTable) do 			
			if (func~=nil) then 		
				local status, err = pcall(func, ... )
				if (status==false) then 
					local err_text = string.format("event [%s]-> %s: %s ",name,tostring(uid), tostring(err))
					system.error(err_text)
					break
				end 
			end 			
		end 
	end 
end 


--  Subscribes to an event 
function event.subscribe(evt, name, func) 
	local fenv = getfenv(func) 
	if subscriptions[fenv]==nil then 
		subscriptions[fenv] = {}
	end 
	local envEvents = subscriptions[fenv]
	if envEvents[evt] == nil then 
		envEvents[evt] = {}
	end
	local sysEvents = envEvents[evt] 
	sysEvents[name] = func 
end 

-- unsubscribes from an event 
function event.unsubscribe(evt, name) 
	local fenv = getfenv(func) 
	print(fenv,_G)
	if subscriptions[fenv]==nil then 
		subscriptions[fenv] = {}
	end 
	local envEvents = subscriptions[fenv]
	if envEvents[evt] == nil then 
		envEvents[evt] = {}
	end
	local sysEvents = envEvents[evt] 
	sysEvents[name] = nil
end 



