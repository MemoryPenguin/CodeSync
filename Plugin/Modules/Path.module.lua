local Path = {}

function Path.Split(pathStr)
	local parts = {}
	
	for chunk in pathStr:gmatch("[^%.]+") do
		table.insert(parts, chunk)
	end
	
	return parts
end

function Path.IsValid(pathStr, start)
	start = start or game
	
	local parts = Path.Split(pathStr)
	
	for _, part in ipairs(parts) do
		-- skip redundant 'game' if it's the first part
		if start == game and part ~= "game" or start ~= game then
			start = start:FindFirstChild(part)
			
			if not start then
				return false
			end
		end
	end
	
	return true
end

function Path.CreateTree(pathStr, start, ignoreLast, instanceType)
	instanceType = instanceType or "Folder"
	start = start or game
	
	local parts = Path.Split(pathStr)
	
	for index, part in ipairs(parts) do
		-- skip redundant 'game' if it's the first part
		if start == game and part ~= "game" or start ~= game then
			local nextObj = start:FindFirstChild(part)
			
			if not nextObj and (ignoreLast and index ~= #parts or not ignoreLast) then
				nextObj = Instance.new(instanceType, start)
				nextObj.Name = part
			end
			
			if nextObj then
				start = nextObj
			end
		end
	end
	
	return start
end

function Path.GetObjectAt(pathStr, start)
	start = start or game
	
	local parts = Path.Split(pathStr)
	
	for _, part in ipairs(parts) do
		start = start:FindFirstChild(part)
		
		if not start then
			return nil
		end
	end
	
	return start
end

function Path.OSToROBLOXPath(ospath)
	local path = ospath:gsub("%.[%.%w]+$", "") -- remove extensions
	path = path:gsub("[\\/]", "%.") -- change separators to .
	
	return path
end

return Path