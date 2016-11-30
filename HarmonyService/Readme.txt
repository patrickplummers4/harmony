<p>
    &nbsp;</p>
<p>
    This is a RESTful service implementation of the Harmony Console app.&nbsp;&nbsp; To install it as a Windows service, run the InstallService.bat file from a command window (cmd.exe) running as Administrator.</p>
<p>
    To run the service as a console app in Visual Studio 2013 (debug mode), the easiest thing to do is run Visual Studio as an Administrator, select the HarmonyService project as the startup project and make sure that &quot;console&quot; is passed as a command line argument (Project Properties -&gt; Debug).&nbsp;&nbsp; The reason for running as Administrator is so that the WCF service can self-register on the port defined in the app.config.&nbsp;&nbsp; If you don&#39;t wish to do this, you&#39;ll need to use the netsh windows command to assign the appropriate rights.</p>
<p>
    Open a command window as an Administrator:</p>
<p>
    Determine free ports:</p>
<p>
    netsh http show urlacl</p>
<p>
    See command line options:</p>
<p>
    netsh http add urlacl help</p>
<p>
    Common command for this service:</p>
<p>
    netsh http add urlacl <a href="http://+:8085/HarmonyService">http://+:8085/HarmonyService</a> user=domain\user</p>
<p>
    &nbsp;</p>
<p>
    &nbsp;</p>
