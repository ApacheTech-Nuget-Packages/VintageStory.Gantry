﻿using Vintagestory.API.Server;

namespace Gantry.Core.ModSystems.Abstractions;

/// <summary>
///     Base representation of a ModSystem used to extend Vintage Story.
/// </summary>
/// <seealso cref="GantrySubsystem" />
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public abstract class GantrySubsystem : GantrySubsystemBase
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="ModSystemBase"/> class.
    /// </summary>
    protected GantrySubsystem()
    {
    }

    /// <summary>
    ///     Common API Components that are available on the server and the client.<br/>
    ///     Cast to ICoreServerAPI, or ICoreClientAPI, to access side specific features.
    /// </summary>
    public ICoreAPI UApi = default!;

    /// <summary>
    ///     Called during initial mod loading, called before any mod receives the call to Start().
    /// </summary>
    /// <param name="api">
    ///     Common API Components that are available on the server and the client.<br/>
    ///     Cast to ICoreServerAPI or ICoreClientAPI to access side specific features.
    /// </param>
    public override void StartPre(ICoreAPI api)
    {
        UApi = api;
        StartPreUniversal(api);
        switch (api)
        {
            case ICoreClientAPI capi:
                StartPreClientSide(capi);
                break;
            case ICoreServerAPI sapi:
                StartPreServerSide(sapi);
                break;
        }
    }

    /// <summary>
    ///     If you need mods to be executed in a certain order, adjust these methods return value.<br/>
    ///     The server will call each Mods Start() method the ascending order of each mod's execute order value.<br/>
    ///     And thus, as long as every mod registers it's event handlers in the Start() method, all event handlers<br/>
    ///     will be called in the same execution order.<br/>
    ///     Default execute order of some survival mod parts.<br/><br/>
    /// 
    ///     World Gen:<br/>
    ///     - GenTerra: 0<br/>
    ///     - RockStrata: 0.1<br/>
    ///     - Deposits: 0.2<br/>
    ///     - Caves: 0.3<br/>
    ///     - BlockLayers: 0.4<br/><br/>
    /// 
    ///     Asset Loading:<br/>
    ///     - Json Overrides loader: 0.05<br/>
    ///     - Load hardcoded mantle block: 0.1<br/>
    ///     - Block and Item Loader: 0.2<br/>
    ///     - Recipes (Smithing, Knapping, ClayForming, Grid recipes, Alloys) Loader: 1
    /// </summary>
    public override double ExecuteOrder()
    {
        return 0.05;
    }
}