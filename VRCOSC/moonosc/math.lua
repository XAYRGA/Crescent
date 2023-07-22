Vector = {}
Vector.__index = Vector


function ToLuaVector(hmdVec)
    return Vector(hmdVec.X, hmdVex.Y, hmdVec.Z)
end

function Vector.__add(a, b)
  if type(a) == "number" then
    return Vector.new(b.x + a, b.y + a, b.z + a)
  elseif type(b) == "number" then
    return Vector.new(a.x + b, a.y + b, a.z + b)
  else
    return Vector.new(a.x + b.x, a.y + b.y, a.z + b.z)
  end
end

function Vector.__sub(a, b)
  if type(a) == "number" then
    return Vector.new(a - b.x, a - b.y, a - b.z)
  elseif type(b) == "number" then
    return Vector.new(a.x - b, a.y - b, a.z - b)
  else
    return Vector.new(a.x - b.x, a.y - b.y, a.z - b.z)
  end
end

function Vector.__mul(a, b)
  if type(a) == "number" then
    return Vector.new(b.x * a, b.y * a)
  elseif type(b) == "number" then
    return Vector.new(a.x * b, a.y * b)
  else
    return Vector.new(a.x * b.x, a.y * b.y)
  end
end

function Vector.__div(a, b)
  if type(a) == "number" then
    return Vector.new(a / b.x, a / b.y, a / b.z)
  elseif type(b) == "number" then
    return Vector.new(a.x / b, a.y / b, a.z /b)
  else
    return Vector.new(a.x / b.x, a.y / b.y, a.z / b.z)
  end
end

function Vector.__eq(a, b)
  return a.x == b.x and a.y == b.y and a.z == b.z
end

function Vector.__le(a, b)
  return a.x <= b.x and a.y <= b.y and a.z <= b.z
end

function Vector.__tostring(a)
  return "(" .. a.x .. ", " .. a.y .. "," .. a.z ..")"
end

function Vector.new(x, y)
  return setmetatable({ x = x or 0, y = y or 0, z = z or 0}, Vector)
end

function Vector.distance(a, b)
  return (b - a):len()
end

function Vector:clone()
  return Vector.new(self.x, self.y, self.z)
end

function Vector:unpack()
  return self.x, self.y, self.z
end

function Vector:len()
  return math.sqrt(self.x * self.x + self.y * self.y + self.z * self.z)
end

function Vector:lenSq()
  return self.x * self.x + self.y * self.y + self.z * self.z
end

function Vector:normalize()
  local len = self:len()
  self.x = self.x / len
  self.y = self.y / len
  self.z = self.z / len
  return self
end

function Vector:normalized()
  return self / self:len()
end

function Vector:projectOn(other)
  return (self * other) * other / other:lenSq()
end

setmetatable(Vector, { __call = function(_, ...) return Vector.new(...) end })