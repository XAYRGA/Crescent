AUTHOR = "xayrga" -- Name of YOU :D 
NAME = "test plugin" -- Name of your plugin
GUID = "460d14f6-90a3-44e1-9ee6-bd945e1fb66c" -- Every plugin must have a GUID  

-- Plugin API syntax is Lua 5.2 

local debounce = 0 
event.subscribe("update","myUpdate",function() -- Subscribing to the update event (runs at 60hz)
	local head = ovr.getHMD() -- Get the ID of the HMD 
	local trackerPos = ovr.getTrackerPosition(head) -- Get the pos of the head 
	local trackerAng = ovr.getTrackerRotation(head) -- Get the rotation 
	local tilt = trackerAng.Y -- Extract the Y component
	local mils = tonumber(string.format("%.2f", tilt)) -- Truncate decimal

	if (debounce~=mils) then  -- A little debouncing logic
		debounce = mils  
		print(mils/1.5)
		avatar.setFloat("StaticBase",math.abs(mils/1.1)) -- Set the avatar variable. 
		--vrc.input.jump()
	end 
end)

