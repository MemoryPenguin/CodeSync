local Accessor = require(script.Parent.Accessor)
local Path = require(script.Parent.Path)
local ScriptHelper = require(script.Parent.ScriptHelper)
local ScriptManager = require(script.Parent.ScriptManager)

local SyncManager = {}
SyncManager.__index = SyncManager

SyncManager.StopCauses = {
	TREE_MODIFIED = "The tree was modified unexpectedly",
	HTTP_ERROR = "Couldn't reach the HTTP server",
	BAD_HTTP_RESPONSE = "Got an unsupported or malformed HTTP response",
	USER = "Stopped by the user",
	MISC = "Unexpected Lua error"
}

SyncManager.RequestInterval = 5

function SyncManager.new(port, targetStr)
	local self = setmetatable({}, SyncManager)
	self.Accessor = Accessor.new(port)

	local target = Path.CreateTree(targetStr)
	self.ScriptManager = ScriptManager.new(target)

	target.AncestryChanged:connect(function()
		self:Stop(SyncManager.StopCauses.TREE_MODIFIED)
	end)

	target.Changed:connect(function()
		self:Stop(SyncManager.StopCauses.TREE_MODIFIED)
	end)

	self.Syncing = false
	return self
end

function SyncManager:Start(stopCallback)
	local startTick = tick()
	self.Syncing = startTick
	self.StopCallback = stopCallback

	local list = self.Accessor:GetFileList()

	if not list then
		self:Stop(SyncManager.StopCauses.HTTP_ERROR)
		return
	end

	for _, filePath in ipairs(list) do
		local contents = self.Accessor:ReadFile(filePath)

		if not contents then
			self:Stop(SyncManager.StopCauses.HTTP_ERROR)
			return
		end

		self:AddFile(filePath)
		self:UpdateFile(filePath, contents)
	end

	spawn(function()
		while self.Syncing == startTick do
			wait(SyncManager.RequestInterval)
			local changes = self.Accessor:GetChangedFiles()
			if not changes then
				print("inval change")
				self:Stop(SyncManager.StopCauses.HTTP_ERROR)
				return
			end

			print(changes)
			print(#changes)

			for _, change in ipairs(changes) do
				print("CHANGE: "..change.Path..", TYPE: "..change.Type)
				-- modify
				if change.Type == 0 then
					local rbxPath = Path.OSToROBLOXPath(change.Path)

					if not self.ScriptManager:GetObject(rbxPath) then
						print("Creating missing object for path "..change.Path)
						self:AddFile(change.Path)
					end

					local contents = self.Accessor:ReadFile(change.Path)

					if not contents then
						self:Stop(SyncManager.StopCauses.HTTP_ERROR)
						return
					else
						self:UpdateFile(change.Path, contents)
					end
				-- delete
				elseif change.Type == 1 then
					self:RemoveFile(change.Path)
				else
					self:Stop(SyncManager.StopCauses.BAD_HTTP_RESPONSE)
					return
				end
			end
		end
	end)
end

function SyncManager:Stop(reason)
	reason = reason or SyncManager.StopCauses.USER
	self.Syncing = false

	print("SyncManager is stopping sync (reason: "..reason..")")

	if self.SyncCallback then
		self.SyncCallback(reason)
	end
end

function SyncManager:AddFile(file)
	local rbxPath = Path.OSToROBLOXPath(file)
	local objType = ScriptHelper.GetType(file)
	self.ScriptManager:CreateObject(rbxPath, objType)
end

function SyncManager:UpdateFile(file, contents)
	local rbxPath = Path.OSToROBLOXPath(file)
	self.ScriptManager:SetObjectSource(rbxPath, contents)
end

function SyncManager:RemoveFile(file)
	local rbxPath = Path.OSToROBLOXPath(file)
	self.ScriptManager:RemoveObject(rbxPath)
end

return SyncManager
