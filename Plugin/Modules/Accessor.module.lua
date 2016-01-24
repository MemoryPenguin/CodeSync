local HttpService = game:GetService("HttpService")

local Accessor = {}
Accessor.__index = Accessor

Accessor.HOST = "http://localhost:%d/"

function Accessor.new(port)
	local self = setmetatable({}, Accessor)
	self.Port = port
	
	return self
end

function Accessor:GetFromServer(requestString)
	local url = Accessor.HOST:format(self.Port)..requestString
	
	local succeeded, result = pcall(function()
		return HttpService:GetAsync(url)
	end)
	
	if not succeeded then
		--warn("Couldn't access the local server at port "..self.Port.."; ensure it is running and HttpService.HttpEnabled is true.", 0)
		return false
	else
		return result
	end
end

function Accessor:GetJson(request)
	local json = self:GetFromServer(request)
	
	if json then
		return HttpService:JSONDecode(json)
	end
end

function Accessor:GetFileList()
	return self:GetJson("list")
end

function Accessor:GetSyncInfo()
	return self:GetJson("info")
end

function Accessor:GetChangedFiles()
	return self:GetJson("changes")
end

function Accessor:ReadFile(file)
	return self:GetFromServer("read?file="..file)
end

function Accessor.TryReach(port)
	return pcall(function()
		return HttpService:JSONDecode(HttpService:GetAsync(Accessor.HOST:format(port).."info"))
	end)
end

return Accessor