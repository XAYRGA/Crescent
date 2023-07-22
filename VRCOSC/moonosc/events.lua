local subscriptions = {}
local pairs = pairs 
local getfenv = getfenv 
local pcall = pcall 

event = {}


function event.RaiseGlobal(name , ... ) 
	for environment,events in pairs(subscriptions) do 
		if events[name] ~= nil then 	
			local evtTable = events[name]
			for uid, func in pairs(evtTable) do 			
				if (func~=nil) then 		
					local status, err = pcall(func, ... )
					if (status==false) then 
						print(tostring(err))
						break
					end 
				end 			
			end 
		end 
	end 
end 



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



