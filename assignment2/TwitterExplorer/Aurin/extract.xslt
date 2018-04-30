<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
                xmlns="http://schemas.datacontract.org/2004/07/AnotateWithAurin"
                xmlns:wfs="http://www.opengis.net/wfs"
                xmlns:gml="http://www.opengis.net/gml"
                xmlns:aurin="http://www.aurin.org.au"
                xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl">

  <xsl:output method="xml" indent="yes" />


  <xsl:template match="/">
    <CensusMedians xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
      <Features>
        <xsl:apply-templates select='//gml:featureMember' />
      </Features>
    </CensusMedians>
  </xsl:template>


  <xsl:template match="gml:featureMember">
    <Feature>
      <BoundaryDescription>
        <xsl:value-of select='.//gml:boundedBy//gml:coordinates' />
      </BoundaryDescription>
      <LocationDescription>
        <xsl:apply-templates select='.//gml:outerBoundaryIs' />
      </LocationDescription>

      <Parameters>
        <Id>
          <xsl:value-of select='.//aurin:sa4_code16|.//aurin:sa2_main16' />
        </Id>
        <Income>
          <xsl:value-of select='.//aurin:median_tot_prsnl_inc_weekly' />
        </Income>
        <Name>
          <xsl:value-of select='.//aurin:sa4_name16|.//aurin:sa2_name16' />
        </Name>
      </Parameters>

    
    </Feature>
  </xsl:template>


  <xsl:template match="gml:outerBoundaryIs">
    <Polygon>
      <xsl:value-of select='.//gml:coordinates' />
    </Polygon>
  </xsl:template>


  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>