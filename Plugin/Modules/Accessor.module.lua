local HttpService = game:GetService("HttpService")

local Accessor = {}
Accessor.__index = Accessor

Accessor.HOST = "http://localhost:%d/"
Accessor.NUM_TRIES = 2
Accessor.LIMIT_COOLDOWN = 15

function Accessor.new(port)
	local self = setmetatable({}, Accessor)
	self.Port = port

	return self
end

local function Try(port, request)
	local url = Accessor.HOST:format(port)..request
	local tries = 0

	while tries < Accessor.NUM_TRIES do
		local succeeded, result = pcall(function()
			return HttpService:GetAsync(url)
		end)

		if not succeeded then
			if result ~= "Number of requests exceeded limit" then
				print("http err trying again")
				tries = tries + 1
			else
				print("[CodeSync] HttpService request limit has been reached; yielding for "..Accessor.LIMIT_COOLDOWN.." seconds before retrying.")
				wait(Accessor.LIMIT_COOLDOWN)
			end
		else
			return result
		end
	end

	return false
end

function Accessor:GetFromServer(requestString)
	return Try(self.Port, requestString)
end

function Accessor:GetJson(request)
	local json = self:GetFromServer(request)

	if json then
		print(json)
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
		return HttpService:JSONDecode(Try(port, "info"))
	end)
end

return Accessor
