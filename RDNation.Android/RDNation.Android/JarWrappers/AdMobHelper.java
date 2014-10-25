/*
 * Copyright (C) 2012 James Montemagno (motz2k1@oh.rr.com) http://www.montemagno.com
 * 
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */

package RDNation.Droid.admob;//this is where it packaged and your .cs file will need to reference

import android.view.View;
import com.google.ads.*;

public class AdMobHelper
{
	private AdMobHelper() { }

	//this method will refresh the ad, you can add more to the AdRequest if you want
	public static void loadAd(View view)
	{
		((AdView)view).loadAd(new AdRequest());
	}
	
	//destroys the add, should be called in the override of the destory in the activity.
	public static void destroy(View view)
	{
		((AdView)view).destroy();
	}
}