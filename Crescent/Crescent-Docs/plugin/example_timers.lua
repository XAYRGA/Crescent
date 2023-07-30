AUTHOR = "xayrga" -- Name of YOU :D 
NAME = "Timer Example Plugin" -- Name of your plugin
GUID = "fb3cdd2e-cb4e-4376-bb0e-69585526e302" -- Every plugin must have a GUID  

-- Plugin API syntax is Lua 5.2 

-- This timer repeats every 1.5 seconds
-- It runs for 0 times (forever) 
-- and calls the function  below
timer.create("JumpEvery1&1/2Seconds",1.5,0,function()
	vrc.input.jump(true)
	timer.simple(0.1,function() -- we also need to stop the jump input,
	--we'll create one of these every time we execute, 
	--so we can use a simple timer.
		vrc.input.jump(false)
	end)
	print("JUMPY")
end)

