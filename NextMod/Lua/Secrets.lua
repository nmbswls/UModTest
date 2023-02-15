local Secrets = {}

local refSecretsSystem = CS.FirstBepinPlugin.SecretsSystem.Instance;
function Secrets.TestFunc(runner,env)
    runner.MyCustom()
end

function Secrets.JiesuanKou(runner,env)
	local val = runner.callCommand.ParamList[2];
    runner.SecretJieSuan(0, val);
end


function Secrets.IsSecretOpen(env)
	return refSecretsSystem:GetPlayerLevel() > 0;
end

return Secrets
