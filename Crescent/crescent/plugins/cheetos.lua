AUTHOR = "xayrga"
NAME = "hands over head"
GUID = "1a07395a-4722-4769-95ea-f5bda763f683"

local debounce = 0 

local BUTTON_A = 2 
-- Coordinate X = Left Right, Y = Up Down , Z = Forward Back
event.subscribe("update","myUpdate",function()
	
	local lHandPos = ovr.getTrackerPosition( ovr.getLeftHand() )
	local rHandPos = ovr.getTrackerPosition( ovr.getRightHand() )
	local HMDPos = ovr.getTrackerPosition( ovr.getHMD() )

	-- Check if the player has their hands over their head. 
	if ( (lHandPos.Y > HMDPos.Y) and (rHandPos.Y > HMDPos.Y) ) then 

		local dist = lHandPos.Y - HMDPos.Y

		print("left hand is above head!!! " .. dist )
		-- Writes the "Expression-Scared parameter to your avatar as true."
		avatar.setBool("Expression-Scared",true)
	end 

end)

