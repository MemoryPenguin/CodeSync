local plugin = plugin -- squelch warnings

local Modules = script.Parent.Modules

local Accessor = require(Modules.Accessor)
local Path = require(Modules.Path)
local SyncManager = require(Modules.SyncManager)
local UI = require(Modules.UI)

local Toolbar = plugin:CreateToolbar("CodeSync")
local Button = Toolbar:CreateButton("CodeSync", "Allows you to synchronize scripts from your computer to ROBLOX.", "rbxassetid://347676411")

local Open = false

Button.Click:connect(function()
	Open = not Open
	Button:SetActive(Open)
	if Open then
		UI:Show()
	else
		UI:Hide()
	end
end)

local CurrentManager = nil
local InfoFormat = "%s\nSource: %s\nTarget: %s"

UI.ToggleButton.MouseButton1Click:connect(function()
	UI.FreezeButton()

	if not CurrentManager then
		UI.FreezePortBox()
		local port = UI.GetPort()
		UI.SetInfoText("Testing port...")
		local success, info = Accessor.TryReach(port)
		
		if success then
			UI.SetInfoText("Starting sync...")
			
			CurrentManager = SyncManager.new(port, info.Target)
			CurrentManager:Start(function(reason)
				if reason ~= SyncManager.StopReasons.USER then
					UI.SetInfoText(("Got an error from the sync manager:\n%s"):format(reason))
				end
			end)
			
			print("Started sync.")
			UI.ToggleButton.Text = "Stop sync"
			UI.SetInfoText(InfoFormat:format("Currently syncing", info.Source, info.Target))
		else
			UI.ThawPortBox()
			UI.SetInfoText("Can't reach server on port "..port..", please ensure HttpService.HttpEnabled is true and double-check your configuration.")
		end
	else
		CurrentManager:Stop()
		print("Stopped sync.")
		UI.ToggleButton.Text = "Start sync"
		UI.CheckPort()
		CurrentManager = nil
		UI.ThawPortBox()
	end
	
	UI.ThawButton()
end)