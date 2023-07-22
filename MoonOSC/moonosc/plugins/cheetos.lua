AUTHOR = "xayrga"
NAME = "test plugin"

local debounce = 0 
event.subscribe("update","myUpdate",function()
	local head = ovr.getHMD()
	local trackerPos = ovr.getTrackerPosition(head)
	local trackerAng = ovr.getTrackerRotation(head) 
	local tilt = trackerAng.Y
	local mils = tonumber(string.format("%.2f", tilt))

		if (debounce~=mils) then 
			debounce = mils  
			print(mils/1.5)
			avatar.setFloat("StaticBase",math.abs(mils/1.1))
			--vrc.input.jump()
		end 

end)




