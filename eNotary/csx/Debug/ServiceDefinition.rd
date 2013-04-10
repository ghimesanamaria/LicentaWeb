<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="eNotary" generation="1" functional="0" release="0" Id="7a462520-997a-451f-8cd2-1228dce75d40" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="eNotaryGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="eNotaryWebRole:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/eNotary/eNotaryGroup/LB:eNotaryWebRole:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="eNotaryWebRole:EmailAdmin" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:EmailAdmin" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:eNotarySpace" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:eNotarySpace" />
          </maps>
        </aCS>
        <aCS name="eNotaryWebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/eNotary/eNotaryGroup/MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
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
      </channels>
      <maps>
        <map name="MapeNotaryWebRole:EmailAdmin" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/EmailAdmin" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:eNotarySpace" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/eNotarySpace" />
          </setting>
        </map>
        <map name="MapeNotaryWebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
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
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="EmailAdmin" defaultValue="" />
              <aCS name="eNotarySpace" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;eNotaryWebRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;eNotaryWebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="eNotarySpace" defaultAmount="[30000,30000,30000]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
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
    <implementation Id="15795261-93f3-47c2-96f5-4217284e3c47" ref="Microsoft.RedDog.Contract\ServiceContract\eNotaryContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="84dc91b7-b923-43b9-8c44-4be045986ff7" ref="Microsoft.RedDog.Contract\Interface\eNotaryWebRole:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/eNotary/eNotaryGroup/eNotaryWebRole:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>