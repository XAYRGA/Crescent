timer = {}

--[[
    TODO: This timer code was lifted from my old project MALLET2 
    https://github.com/XAYRGA/Mallet2/blob/master/Mallet/mallet/modules/itimer.lua
    Timers are global and need to be made realm-dependent. 
    Probably entails a whole rewrite of this library. 
]]

local TIMERS = {}
local SIMPLE_TIMERS = {}


local function expect(var,typ,arg,funcnam)
    assert(type(var)==typ, "argument #" .. arg .. " to '" .. funcnam .. "', " .. typ .. " expected, got " .. type(var))
end

function timer.GetTable()
    return TIMERS
end


function timer.create(id,del,reps,func)
    local FUNCN = "create"
    expect(id,"string",1,FUNCN)
    expect(del,"number",2,FUNCN)
    expect(reps,"number",3,FUNCN)
    expect(func,"function",4,FUNCN)

    TIMERS[id] = {
        reps = reps,
        delay = del,
        funct = func,
        paused = false,
        infinite = reps==0,
        start = system.time()
    }
end


function timer.destroy(id) 

end 

function timer.simple(del,func)
    SIMPLE_TIMERS[#SIMPLE_TIMERS + 1] = {
        delay = del,
        funct = func,
        start = system.time()
   } 
end

local function timertick()
    for k,tim in pairs(TIMERS) do
        local elapsed = system.time() - tim.start
         if not tim.paused then 
            if elapsed >= tim.delay then
               local r,e = pcall(tim.funct)
                if not tim.infinite then 
                    tim.reps = tim.reps - 1                
                    if not r then 
                        print(e)
                    end
                    if tim.reps == 0 then 
                        TIMERS[k] = nil
                    end                   
                end           
                tim.start = system.time() 
            end   
        end
    end
    for k,stim in pairs(SIMPLE_TIMERS) do 
        local elapsed = system.time() - stim.start
        if elapsed > stim.delay then 
            stim["funct"]()
            SIMPLE_TIMERS[k] =  nil
        end    
    end
end
event.subscribe("update","timers",timertick)