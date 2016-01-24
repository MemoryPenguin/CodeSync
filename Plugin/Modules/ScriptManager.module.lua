local Path = require(script.Parent.Path)

local ScriptManager = {}
ScriptManager.__index = ScriptManager

function ScriptManager.new(root)
	local self = setmetatable({}, ScriptManager)
	self.Root = root
	
	return self
end

function ScriptManager:GetObject(path)
	return Path.GetObjectAt(path, self.Root)
end

function ScriptManager:CreateObject(path, type)
	if self:GetObject(path) then return self:GetObject(path) end
	
	local parts = Path.Split(path)
	local obj = Instance.new(type)
	
	local parent = Path.CreateTree(path, self.Root, true)
	obj.Parent = parent
	obj.Name = parts[#parts]
	return obj
end

function ScriptManager:RemoveObject(path)
	local object = self:GetObject(path)
	
	if object then
		while #object.Parent:GetChildren() < 2 and object.Parent ~= self.Root do
			object = object.Parent
		end
		
		object:Destroy()
	end
end

function ScriptManager:SetObjectSource(path, source)
	local object = self:GetObject(path)
	
	if object then
		object.Source = source
	end
end

return ScriptManager