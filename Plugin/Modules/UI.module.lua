local CoreGui = game:GetService("CoreGui")

local Accessor = require(script.Parent.Accessor)

local Object = script.CodeSyncUI
local EnabledColor = Color3.new(34 / 255, 162 / 255, 147 / 255)
local DisabledColor = Color3.new(162 / 255, 162 / 255, 162 / 255)

local UI = {}
UI.PortBox = Object.Container.Contents.PortBox.Input
UI.ToggleButton = Object.Container.Contents.Toggle
UI.Info = Object.Container.Contents.Info

function UI.Show()
	Object.Parent = CoreGui
end

function UI.Hide()
	Object.Parent = script
end

function UI.IsVisible()
	return Object.Parent == CoreGui
end

function UI.SetInfoText(text)
	Object.Container.Contents.Info.Text = text
end

function UI.GetInfoText()
	return Object.Container.Contents.Info.Text
end

function UI.GetPort()
	return tonumber(Object.Container.Contents.PortBox.Input.Text)
end

function UI.FreezeButton()
	UI.ToggleButton.Active = false
	UI.ToggleButton.AutoButtonColor = false
	UI.ToggleButton.BackgroundColor3 = DisabledColor
end

function UI.ThawButton()
	UI.ToggleButton.Active = true
	UI.ToggleButton.AutoButtonColor = true
	UI.ToggleButton.BackgroundColor3 = EnabledColor
end

function UI.FreezePortBox()
	UI.PortBox.Active = false
	UI.PortBox.BackgroundColor3 = DisabledColor
	UI.PortBox:ReleaseFocus()
end

function UI.ThawPortBox()
	UI.PortBox.Active = true
	UI.PortBox.BackgroundColor3 = Color3.new(240 / 255, 240 / 255, 240 / 255)
end

function UI.CheckPort()
	local port = tonumber(UI.PortBox.Text)
	
	UI.FreezeButton()
	
	if UI.PortBox.Text == "" then
		UI.SetInfoText("Enter a port to get started.")
	elseif not port or port > 65535 then
		UI.SetInfoText("This port doesn't work! It should be a number between 1 and 65535.")
	else
		UI.ThawButton()
		UI.SetInfoText("Ready to start sync.")
	end
end

UI.PortBox.FocusLost:connect(UI.CheckPort)

UI.CheckPort()

return UI