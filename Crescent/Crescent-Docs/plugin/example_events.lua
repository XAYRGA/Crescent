AUTHOR = "xayrga" -- Name of YOU :D 
NAME = "Event Example Plugin" -- Name of your plugin
GUID = "7d3b082b-d29a-4a9e-b3cf-01ef684bc2eb" -- Every plugin must have a GUID  

-- Plugin API syntax is Lua 5.2 

local falling = true 
event.subscribe("update","myUpdate",function() 
	local fallspeed = avatar.getFloat("VelocityY") -- get the player's vertical velocity.

	if (fallspeed < -10 and falling==false) then
		event.raise("playerEnterFreefall", fallspeed) -- You can raise your own even with whatever logic you want surrounding to it, and even pass data to all of the subscribers
		falling = true 
	elseif (fallspeed > -10 and falling==true) then
		falling = false
		event.raise("playerExitFreefall", fallspeed)
	end
end)

-- Calls when the player starts freefalling 
event.subscribe("playerEnterFreefall","enterFREERF",function(fallspeed)
	print("Now freefalling!")
end)

-- Called when the player stops freefalling
event.subscribe("playerExitFreefall","enterFREERF",function(fallspeed)
	print("Stopped freefalling")
end)