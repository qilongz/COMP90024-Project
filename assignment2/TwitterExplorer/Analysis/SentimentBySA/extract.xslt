<?xml version="1.0" encoding="utf-8"?>


<xsl:stylesheet version="1.0"
                xmlns="http://schemas.datacontract.org/2004/07/SentimentBySA"
                xmlns:wfs="http://www.opengis.net/wfs"
                xmlns:gml="http://www.opengis.net/gml"
                xmlns:aurin="http://www.aurin.org.au"
                xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:g="http://schemas.datacontract.org/2004/07/TwitterUtil.Geo"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl">

  <xsl:output method="xml" indent="yes" />


  <xsl:template match="/">
    <CensusMedians xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
      <Features  xmlns:g="http://schemas.datacontract.org/2004/07/TwitterUtil.Geo">
        <xsl:apply-templates select='//gml:featureMember' />
      </Features>
    </CensusMedians>
  </xsl:template>


  <xsl:template match="gml:featureMember">
    <g:Feature>
      <g:BoundaryDescription>
        <xsl:value-of select='.//gml:boundedBy//gml:coordinates' />
      </g:BoundaryDescription>
      <g:LocationDescription>
        <xsl:apply-templates select='.//gml:outerBoundaryIs' />
      </g:LocationDescription>

      <g:Parameters>
        <g:Id>
          <xsl:value-of select='.//aurin:sa4_code16|.//aurin:sa3_code16|.//aurin:sa2_main16|.//aurin:sa1_7dig16' />
        </g:Id>
        <g:Income>
          <xsl:value-of select='.//aurin:median_tot_prsnl_inc_weekly' />
        </g:Income>
        <g:Name>
          <xsl:value-of select='.//aurin:sa4_name16|.//aurin:sa3_name16|.//aurin:sa2_name16|.//aurin:sa1_main16' />
        </g:Name>
      </g:Parameters>

    
    </g:Feature>
  </xsl:template>


  <xsl:template match="gml:outerBoundaryIs">
    <g:Polygon>
      <xsl:value-of select='.//gml:coordinates' />
    </g:Polygon>
  </xsl:template>


  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>