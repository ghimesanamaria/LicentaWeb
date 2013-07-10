<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="eNotary" generation="1" functional="0" release="0" Id="ad08735a-7568-4fb9-b367-d665118a2904" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="eNotaryGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="eNotaryWebRole:Endpoint1" protocol="https">
          <inToChannel>
            <lBChannelMoniker name="/eNotary/eNotaryGroup/LB:eNotaryWebRole:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/eNotary/eNotaryGroup/LB:eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapCertificate|eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:EmailAdmin" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:EmailAdmin" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:eNotaryCloudStorage" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:eNotaryCloudStorage" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.ActivationToken" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.ActivationToken" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.Administrators" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.Administrators" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainAccountName" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainAccountName" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainControllerFQDN" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainControllerFQDN" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainFQDN" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainFQDN" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainOU" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainOU" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainPassword" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainPassword" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainSiteName" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainSiteName" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.EnableDomainJoin" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.EnableDomainJoin" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.Refresh" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.Refresh" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.Upgrade" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.Upgrade" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.WaitForConnectivity" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.WaitForConnectivity" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:eNotaryWebRole:Endpoint1">
          <toPorts>
            <inPortMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput">
          <toPorts>
            <inPortMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp">
          <toPorts>
            <inPortMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapCertificate|eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" kind="Identity">
          <certificate>
            <certificateMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </certificate>
        </map>
        <map name="MapeNotaryWebRole:EmailAdmin" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/EmailAdmin" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:eNotaryCloudStorage" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/eNotaryCloudStorage" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.ActivationToken" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Connect.ActivationToken" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.Administrators" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Connect.Administrators" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainAccountName" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Connect.DomainAccountName" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainControllerFQDN" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Connect.DomainControllerFQDN" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainFQDN" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Connect.DomainFQDN" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainOU" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Connect.DomainOU" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Connect.DomainPassword" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.DomainSiteName" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Connect.DomainSiteName" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.EnableDomainJoin" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Connect.EnableDomainJoin" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.Refresh" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Connect.Refresh" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.Upgrade" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Connect.Upgrade" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Connect.WaitForConnectivity" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Connect.WaitForConnectivity" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </setting>
        </map>
        <map name="MapeNotaryWebRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/eNotary/eNotaryGroup/eNotaryWebRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="eNotaryWebRole" generation="1" functional="0" release="0" software="C:\Users\Ana\SkyDrive\Documente\LicentaWeb\eNotary\csx\Debug\roles\eNotaryWebRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="https" portRanges="443">
                <certificate>
                  <certificateMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
                </certificate>
              </inPort>
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp" portRanges="3389" />
              <outPort name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/eNotary/eNotaryGroup/SW:eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="EmailAdmin" defaultValue="" />
              <aCS name="eNotaryCloudStorage" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Connect.ActivationToken" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Connect.Administrators" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Connect.DomainAccountName" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Connect.DomainControllerFQDN" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Connect.DomainFQDN" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Connect.DomainOU" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Connect.DomainPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Connect.DomainSiteName" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Connect.EnableDomainJoin" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Connect.Refresh" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Connect.Upgrade" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Connect.WaitForConnectivity" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;eNotaryWebRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;eNotaryWebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="eNotarySpace" defaultAmount="[30000,30000,30000]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/eNotary/eNotaryGroup/eNotaryWebRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/eNotary/eNotaryGroup/eNotaryWebRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/eNotary/eNotaryGroup/eNotaryWebRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="eNotaryWebRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="eNotaryWebRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="eNotaryWebRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="abf419a9-24c3-48a0-a6e0-c98278152a97" ref="Microsoft.RedDog.Contract\ServiceContract\eNotaryContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="e3d18d90-031f-461e-9267-3a0cd9ec26b3" ref="Microsoft.RedDog.Contract\Interface\eNotaryWebRole:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="29d3deea-bb77-4bfa-9cff-1ccd41a2bafc" ref="Microsoft.RedDog.Contract\Interface\eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>