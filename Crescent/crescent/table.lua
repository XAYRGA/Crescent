function table.copy(src)
	local dest = {}
	for k,v in pairs(src) do 
		if type(ref)=="table" then -- deep copy the library
			dest[k] = table.copy(v)
		else 
			dest[k] = v
		end 
	end 
	return dest
end