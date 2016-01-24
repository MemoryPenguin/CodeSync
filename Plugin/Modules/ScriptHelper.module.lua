local ScriptHelper = {}

function ScriptHelper.GetType(filePath)
	local section = filePath:match("%w+%.(%w+)%.%w+")
	
	if section then
		section = section:lower()
	end
	
	if section == "local" then
		return "LocalScript"
	elseif section == "module" then
		return "ModuleScript"
	else
		return "Script"
	end
end

return ScriptHelper