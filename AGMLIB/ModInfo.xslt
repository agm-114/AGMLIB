<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/">
    

    <ModInfo>
      <ModName>AGMLIB</ModName>
      <ModDescription>By: AGM</ModDescription>
      <ModVer>
        <xsl:value-of select="document('')/processing-instruction('param')[text()='Parameter1']"/>
      </ModVer>
      <Assemblies>
        <string>Debug/net481/AGMLIB.dll</string>
        <string>Debug/net481/0Harmony.dll</string>
      </Assemblies>
      <GameVer>0.3.2</GameVer>
    </ModInfo>
  </xsl:template>
</xsl:stylesheet>