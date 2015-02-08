
Add these to your app.config to rebind the moq and nunit.core.interfaces to the correct versions. 

** maybe in the future ill add these automatically but for now there isn't a safe enough solution

<configuration xmlns:xdt="http://schemas.microsoft.com/XML-Document-Transform">
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="Moq" publicKeyToken="69f491c39445e920" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-4.2.1408.717" newVersion="4.2.1408.717" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="nunit.core.interfaces" publicKeyToken="96d09a1eb7f44a77" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-2.6.3.13283" newVersion="2.6.2.12296" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>