// RFIDCommandCenter.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include <iostream>
#include <tm_reader.h>
#include <tmr_params.h>
#include <string>
#include <list>

/*
	Helps convert the ThingMagic Region to a human readable format or from a human readable format to
	a ThingMagic region
*/

class RegionHelper
{
public:
	static std::string const & getRegionName(TMR_Region region)
	{
		switch (region)
		{
		case TMR_Region::TMR_REGION_AR:
		case TMR_Region::TMR_REGION_AU:
		case TMR_Region::TMR_REGION_BD:
		case TMR_Region::TMR_REGION_EU:
		case TMR_Region::TMR_REGION_EU2:
		case TMR_Region::TMR_REGION_EU3:
		case TMR_Region::TMR_REGION_EU4:
		case TMR_Region::TMR_REGION_HK:
		case TMR_Region::TMR_REGION_ID:
		case TMR_Region::TMR_REGION_IN:
		case TMR_Region::TMR_REGION_IS:
		case TMR_Region::TMR_REGION_JP:
		case TMR_Region::TMR_REGION_JP2:
		case TMR_Region::TMR_REGION_JP3:
		case TMR_Region::TMR_REGION_KR:
		case TMR_Region::TMR_REGION_KR2:
		case TMR_Region::TMR_REGION_MO:
		case TMR_Region::TMR_REGION_MY:
		case TMR_Region::TMR_REGION_NA:
		case TMR_Region::TMR_REGION_NA2:
		case TMR_Region::TMR_REGION_NONE:
		case TMR_Region::TMR_REGION_NZ:
		case TMR_Region::TMR_REGION_OPEN:
		case TMR_Region::TMR_REGION_OPEN_EXTENDED:
		case TMR_Region::TMR_REGION_PH:
		case TMR_Region::TMR_REGION_PRC:
		case TMR_Region::TMR_REGION_PRC2:
		case TMR_Region::TMR_REGION_RU:
		case TMR_Region::TMR_REGION_SG:
		case TMR_Region::TMR_REGION_TH:
		case TMR_Region::TMR_REGION_TW:
		case TMR_Region::TMR_REGION_VN:
			break;
		}
	}
	static TMR_Region getRegionFromName(char const * name)
	{
		return TMR_Region::TMR_REGION_NONE;
	}
};

class ITagInfo
{
public:
	enum TagType
	{
		GEN2
	};
	virtual void * getTagData() = 0;
	virtual int getTagDataSize() = 0;

};

class TagInfo : protected ITagInfo
{
public:

};





int main()
{
	return 0;
}
