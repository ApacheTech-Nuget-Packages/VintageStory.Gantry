using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core.DependencyInjection;
using Gantry.Core.DependencyInjection.Registration;
using Gantry.Core.Extensions.GameContent.Gui;
using Gantry.Core.ModSystems;
using Vintagestory.API.Client;
using Gantry.Tests.AcceptanceMod.Features.Gui.Dialogue;

namespace Gantry.Tests.AcceptanceMod.Features.Gui
{
    internal sealed class GuiProgram : ClientModSystem, IClientServiceRegistrar
    {
        public void ConfigureClientModServices(IServiceCollection services)
        {
            services.AddSingleton<TestDialogue>();
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            var dialogue = IOC.Services.Resolve<TestDialogue>();
            api.Input.RegisterGuiDialogueHotKey(dialogue, "Open Acceptance Gui Test Window", GlKeys.U);
        }
    }
}
