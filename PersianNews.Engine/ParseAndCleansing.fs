namespace PersianNews.Engine
open System
open System.Collections.Generic
open System.Linq
open System.IO

module JsonLinesParser =
    open Newtonsoft.Json

    let ToLines (lines:string) = lines.Split([|Environment.NewLine|], StringSplitOptions.RemoveEmptyEntries)

module DataCleanser =
    open System.Text.RegularExpressions
    open System.Net
    let WrappedRegexPattern = @"(\(((.*){0}(.*))\))"
    let CreateWrappedRegex (inner:string) = String.Format (WrappedRegexPattern, inner)
    let ConcatPatterns = Seq.reduce (fun a b -> String.Format ("{0}|{1}", a, b))
    let RemoveTitleAnnotation str =
        let innerWords = [|"+"; @"\+"; "تصویر"; "عکس"; "فیلم"; "ویدئو"; "ویدیو"|]
        let pattern = innerWords |> Array.map CreateWrappedRegex |> ConcatPatterns
        let regex = Regex pattern
        regex.Replace (str, "")

    let IsLetterString (str : string) = str |> Seq.forall (fun c -> Char.IsLetter c)

    let PersianStopWords = [|"ات";"اتفاقا";"اجراست";"احتمالا";"احيانا";"اخ";"اختصارا";"اخر";"اخرها";"اخه";"اخيرا";"ادمهاست";"اراسته";"ارام";"ارزانتر";"ارزومندانه";"ارنه";"اره";"از";"ازادانه";"ازان";"ازانجا";"ازانجاكه";"ازاين";"ازاينرو";"ازبه";"ازجمله";"ازروي";"ازسر";"ازش";"ازقبيل";"ازلحاظ";"ازنظر";"ازو";"ازيك";"اساس";"اساسا";"اسان";"اسانتر";"اساني";"است";"استوارتر";"اسيبپذيرند";"اش";"اشان";"اشتباها";"اشفته";"اشكار";"اشكارا";"اشكارتر";"اشنايند";"اصطلاحا";"اصلا";"اصولا";"اطلاعند";"اغلب";"افزودن";"افسوس";"افقي";"اقل";"اقلا";"اقليت";"اكتسابا";"اكثر";"اكثرا";"اكثريت";"اكنون";"الا";"الاسف";"الان";"البته";"الزاما";"الظاهر";"المقدور";"الهي";"الي";"ام";"اما";"امان";"امرانه";"امروز";"امروزه";"امسال";"امشب";"اميدوارم";"اميدوارند";"اميدواريم";"ان";"انان";"اناني";"انجا";"اند";"اندك";"اندكي";"انرا";"انشاالله";"انقدر";"انكس";"انكه";"انم";"انها";"انهاست";"انوقت";"انچنان";"انچه";"انگار";"انگاه";"انگه";"انگونه";"اهان";"اهاي";"او";"اورد";"اوردن";"اوست";"اول";"اولا";"اولش";"اون";"اونهمه";"اوه";"اويي";"اي";"ايا";"ايشان";"ايم";"اين";"اينان";"اينجا";"اينجاست";"اينجوري";"اينرو";"اينست";"اينطور";"اينقدر";"اينك";"اينكه";"اينها";"اينهاست";"اينهمه";"اينو";"اينچنين";"اينگونه";"اگاهانه";"اگر";"اگرنه";"اگرچه";"اگه";"ب";"با";"بااطمينان";"باانكه";"بااين";"بااينكه";"بار";"باره";"بارها";"بارهاوبارها";"باز";"بازانديشانه";"بازهم";"بازيگوشانه";"باستثناي";"باش";"باشد";"باعلاقه";"بالا";"بالاتر";"بالاخره";"بالاخص";"بالاست";"بالاي";"بالضرور";"بالطبع";"بالعكس";"بالقوه";"بالله";"بالنتيجه";"بالنسبه";"باهم";"باوجود";"باوجودانكه";"باوجوداينكه";"باوجوديكه";"باورند";"بايد";"بايستي";"بتازگي";"بتدريج";"بتمامي";"بجا";"بجاي";"بجز";"بخاطر";"بخاطراينكه";"بخردانه";"بخشه";"بخصوص";"بخوبي";"بد";"بدان";"بدانجا";"بدانها";"بدبينانه";"بدخواهانه";"بدرستي";"بدرشتي";"بدلخواه";"بدون";"بدين";"بدينجا";"بدينسان";"بر";"برا";"براثر";"براساس";"براستي";"بران";"برانند";"برانهاست";"براي";"برايت";"برايش";"برايشان";"برايم";"برايمان";"برحسب";"برخلاف";"برخوردارند";"برخي";"برداشتن";"بردن";"برروي";"برعكس";"برغم";"برمي";"برنمي";"برو";"بروشني";"برپا";"بزعم";"بزودي";"بس";"بسا";"بسادگي";"بسته";"بسختي";"بسرعت";"بسهولت";"بسوي";"بسي";"بسيار";"بسياري";"بشان";"بشدت";"بصورت";"بطبع";"بطور";"بطوركلي";"بطوري";"بطوريكه";"بعد";"بعدا";"بعداز";"بعدازان";"بعدازاين";"بعدازظهر";"بعدها";"بعدي";"بعضي";"بعضيشان";"بعضيها";"بعضيهاشان";"بعضيهايشان";"بعلاوه";"بعيد";"بفهمي";"بقدري";"بكار";"بكرات";"بلادرنگ";"بلافاصله";"بلكه";"بله";"بمراتب";"بموجب";"بموقع";"بنابراين";"بنابه";"بناچار";"بندرت";"به";"بهت";"بهتر";"بهرحال";"بهش";"بهيچ";"بود";"بودن";"بوضوح";"بويژه";"بي";"بيرون";"بيش";"بيشتر";"بين";"بينابين";"بيهوده";"بيگمان";"بپا";"بگرمي";"ت";"تا";"تاانجا";"تاانجاكه";"تاانكه";"تااينجا";"تااينكه";"تابه";"تاجاييكه";"تاحدودي";"تاحدي";"تازه";"تازگي";"تاكنون";"تاوقتي";"تاوقتيكه";"تحت";"تدريج";"تدريجا";"تدريجي";"تر";"ترتيب";"ترجيحا";"ترديد";"ترند";"تري";"ترين";"تصريحا";"تعدادي";"تعمدا";"تفاوت";"تفاوتند";"تفنني";"تقريبا";"تك";"تلويحا";"تمام";"تماما";"تمامشان";"تمامي";"تند";"تنها";"تنهايي";"تنگاتنگ";"تو";"توسط";"تووما";"توي";"تويي";"ث";"ثالثا";"ثاني";"ثانيا";"ج";"جا";"جاي";"جايي";"جبرگرايانه";"جدا";"جدااز";"جداازهم";"جداگانه";"جدي";"جديدا";"جرمزاست";"جز";"جزجز";"جسورانه";"جلو";"جلوتر";"جلوي";"جمع";"جمعا";"جمعي";"جنابعالي";"جنس";"جهت";"جور";"جوري";"ح";"حاشا";"حاشاوكلا";"حاشيه";"حاضر";"حاضرم";"حاكيست";"حال";"حالا";"حالكه";"حالي";"حتما";"حتي";"حداقل";"حداكثر";"حدود";"حرف";"حسابي";"حسابگرانه";"حسب";"حضرتعالي";"حقيرانه";"حقيقتا";"حكما";"حكيمانه";"حول";"خ";"خالص";"خالصانه";"خالي";"خام";"خامسا";"خب";"خداحافظ";"خداست";"خداگونه";"خردمندانه";"خسته";"خشمگين";"خصمانه";"خصوصا";"خلاقانه";"خواسته";"خواه";"خواهد";"خوب";"خوبتر";"خوبست";"خوبي";"خود";"خوداند";"خوداگاهانه";"خودبه";"خودت";"خودتان";"خودتو";"خودرا";"خودش";"خودشان";"خودم";"خودمان";"خودمختارانه";"خودمو";"خودنمايانه";"خودي";"خوش";"خوشبختانه";"خوشبينانه";"خوشحال";"خويش";"خويشتنم";"خوگيرانه";"خير";"خيره";"خيلي";"د";"دا";"داام";"دااما";"داخل";"دادن";"دارا";"داراست";"داراي";"دارد";"داشتن";"داوطلبانه";"دايم";"دايما";"دخترانه";"دراثر";"درازا";"درازاي";"دران";"دراين";"درباب";"درباره";"دربدر";"دربر";"دربرابر";"دربه";"درتخت";"درثاني";"درحال";"درحالي";"درحاليكه";"دردكشان";"درراستاي";"درست";"درسته";"درشت";"درشتي";"درصورتي";"درصورتيكه";"درطي";"درعين";"دركل";"دركنار";"درمجموع";"درمقابل";"درمورد";"درميان";"درنتيجه";"درنهايت";"درهر";"درهرحال";"درهرصورت";"درواقع";"دريغ";"دريغا";"درپي";"دسته";"دشمنيم";"دشوار";"دشوارتر";"دقيق";"دقيقا";"دلخواه";"دلخوش";"دلشاد";"دم";"دهد";"دو";"دوباره";"دوتا";"دوتادوتا";"دور";"دوراز";"دورتر";"دوساله";"دوستانه";"دوم";"دير";"ديرت";"ديرم";"ديروز";"ديروزبه";"ديشب";"ديوانه";"ديوي";"ديگر";"ديگران";"ديگرتا";"ديگري";"ديگه";"دگربار";"دگرباره";"دگرگون";"ذ";"ذاتا";"ذالك";"ذيل";"ذيلا";"ر";"را";"رااز";"رابه";"راجع";"راحت";"راحتر";"رادر";"راسا";"راست";"راستا";"راستي";"رسما";"رشته";"رغم";"رفتارهاست";"رنجند";"رندانه";"رهگشاست";"رو";"رواست";"روبرو";"روبروست";"روبه";"روز";"روزانه";"روزبروز";"روزمره";"روش";"روشن";"روشني";"روي";"رويش";"رياكارانه";"ريز";"ريزان";"ز";"زد";"زدن";"زده";"زشت";"زماني";"زنند";"زهي";"زو";"زود";"زودتر";"زودي";"زياد";"زيادتر";"زياده";"زيبا";"زيباتر";"زير";"زيرا";"زيراكه";"زيركانه";"زيرند";"زيرچشمي";"زين";"س";"سابقا";"ساختن";"ساده";"سادگي";"سازان";"سازهاست";"سازي";"سازگارانه";"ساكنند";"سالانه";"سالته";"سالم";"سالهاست";"ساليانه";"ساير";"سايران";"سايرين";"ست";"سخت";"سختتر";"سخته";"سر";"سراسر";"سرانجام";"سراپا";"سري";"سريع";"سريعا";"سعادتمندانه";"سنگدلانه";"سنگين";"سه";"سهوا";"سوم";"سياه";"سيخ";"سپس";"ش";"شااالله";"شاد";"شادتر";"شادمان";"شاكله";"شان";"شاهدند";"شاهديم";"شايد";"شبانه";"شبهاست";"شتابان";"شتابزده";"شجاعانه";"شخصا";"شد";"شدت";"شدن";"شده";"شديدا";"شما";"شماري";"شماست";"شمايند";"شود";"شوراست";"شوقم";"شيرين";"شيرينه";"شيك";"ص";"صادقانه";"صاف";"صد";"صدالبته";"صددرصد";"صراحتا";"صرفا";"صريح";"صريحا";"صريحتر";"صميمانه";"صندوق";"ض";"ضرورتا";"ضعيف";"ضعيفتر";"ضمن";"ضمنا";"ط";"طبعا";"طبيعتا";"طلبكارانه";"طور";"طوري";"طي";"ظ";"ظاهرا";"ع";"عاجزانه";"عادلانه";"عاقبت";"عاقلانه";"عالمانه";"عالي";"عبارتند";"عجب";"عجولانه";"عرفاني";"عزيز";"عقب";"عقبتر";"علاوه";"علنا";"عليرغم";"عليه";"عمدا";"عمدتا";"عمده";"عمدي";"عملا";"عملي";"عملگرايانه";"عموم";"عموما";"عميقا";"عن";"عنقريب";"عينا";"غ";"غالبا";"غزالان";"غير";"غيراز";"غيرازان";"غيرازاين";"غيرتصادفي";"غيرطبيعي";"غيرعمدي";"غيرمستقيم";"غيريكسان";"ف";"فاقد";"فبها";"فر";"فراتر";"فراتراز";"فراوان";"فردا";"فعالانه";"فعلا";"فقط";"فكورانه";"فلان";"فلذا";"فناورانه";"فهرستوار";"فورا";"فوري";"فوق";"ق";"قاالند";"قابل";"قاطبه";"قاطعانه";"قاعدتا";"قانونا";"قبل";"قبلا";"قبلند";"قد";"قدر";"قدرمسلم";"قدري";"قرار";"قراردادن";"قريب";"قضاياست";"قطعا";"قيلا";"ك";"كارافرينانه";"كاربرمدارانه";"كارند";"كاش";"كاشكي";"كاملا";"كاملتر";"كان";"كاين";"كجا";"كجاست";"كدام";"كدامند";"كداميك";"كرات";"كرد";"كردن";"كرده";"كز";"كزين";"كس";"كساني";"كسي";"كشيدن";"كل";"كلا";"كلي";"كليشه";"كم";"كمااينكه";"كمابيش";"كماكان";"كمتر";"كمتره";"كمي";"كنار";"كنارش";"كنان";"كنايه";"كند";"كنم";"كنند";"كننده";"كه";"كودكانه";"كوركورانه";"كي";"كيست";"ل";"لا";"لااقل";"لاجرم";"لب";"لذا";"لزوما";"لطفا";"ليكن";"م";"ما";"مات";"مادام";"مادامي";"ماداميكه";"ماست";"ماشينوار";"ماقبل";"مالا";"مالامال";"مامان";"مانند";"ماهرانه";"ماهيتا";"مايي";"مبادا";"متاسفانه";"متعاقبا";"متفاوتند";"متفكرانه";"متقابلا";"متوالي";"متوسفانه";"مثابه";"مثل";"مثلا";"مجبورند";"مجددا";"مجرمانه";"مجموع";"مجموعا";"محتاجند";"محتاط";"محتاطانه";"محكم";"مخالفند";"مختصر";"مختصرا";"مخصوصا";"مدام";"مداوم";"مدبر";"مدبرانه";"مدتهاست";"مدتي";"مرا";"مراتب";"مرتب";"مرتبا";"مردانه";"مستحضريد";"مستعد";"مستقلا";"مستقيم";"مستقيما";"مستمر";"مستمرا";"مستند";"مسلم";"مسلما";"مسيولانه";"مشت";"مشتركا";"مشخص";"مشغول";"مشغولند";"مشكل";"مشكلتر";"مطلق";"مطلقا";"مطمانا";"مطمانم";"مطمنا";"مع";"معتقدم";"معتقدند";"معتقديم";"معدود";"معذوريم";"معلق";"معلومه";"معمولا";"معمولي";"مغرضانه";"مغلوب";"مفيدند";"مقدار";"مقصر";"مقصري";"مقلوب";"مكرر";"مكررا";"ملزم";"مميزيهاست";"من";"منتهي";"منحصر";"منحصرا";"منحصربفرد";"منصفانه";"منطقا";"منطقي";"مني";"مواجهند";"موجب";"موجودند";"موخر";"مورد";"موقعيكه";"مي";"ميان";"ميزان";"مگر";"مگرانكه";"مگراينكه";"ن";"نااميد";"نااگاهانه";"ناتوان";"ناخواسته";"ناخوانده";"ناديده";"ناراضي";"ناسازگارانه";"ناشناخته";"ناكام";"ناهشيار";"ناپذير";"ناچار";"ناگزير";"ناگهان";"نبايد";"نبش";"نتيجتا";"نخست";"نخودي";"ندارد";"ندرت";"ندرتا";"نرم";"نرمي";"نزد";"نزديك";"نزديكتر";"نسبت";"نظرا";"نظربه";"نظير";"نفرند";"نفهمي";"نقادانه";"نمي";"نه";"نهان";"نهايت";"نهايتا";"نواورانه";"نوع";"نوعا";"نوميد";"نيازمندانه";"نيازمندند";"نيز";"نيست";"نيك";"نيمي";"ه";"ها";"هاست";"هان";"هايي";"هدف";"هر";"هرازچندگاهي";"هرانچه";"هرجا";"هرزچندگاهي";"هرساله";"هرقدر";"هركدام";"هركس";"هركسي";"هركه";"هروقت";"هرچقدر";"هرچند";"هرچه";"هرگاه";"هرگز";"هرگونه";"هزارها";"هست";"هستند";"هشيارانه";"هق";"هم";"همان";"همانا";"همانطور";"همانطوركه";"همانطوري";"همانطوريكه";"همانقدر";"همانند";"همانها";"هماني";"هماهنگتر";"همخوان";"همدلانه";"همديگر";"همزمان";"همسو";"همسوبا";"همكارانه";"همنوا";"همه";"همواره";"همي";"هميشه";"هميشگي";"همين";"همينطور";"همينطوركه";"همينطوري";"همينطوريكه";"همينكه";"همچنان";"همچنانكه";"همچنين";"همچون";"همچين";"همگام";"همگان";"همگي";"هنوز";"هنگاميكه";"هوشمندانه";"هوشيارانه";"هوي";"هي";"هيچ";"هيچكدام";"هيچكس";"هيچي";"هيچيك";"هيچگاه";"هيچگونه";"وابسته";"واضح";"واضحتر";"واقع";"واقعا";"واقعي";"واقفند";"واي";"وجه";"وجود";"وجوديكه";"وحشت";"ور";"ورا";"وراي";"وزو";"وضوح";"وقتي";"وقتيكه";"ولي";"وليكن";"وهمين";"وي";"ويا";"ويژه";"وگر";"وگرنه";"يا";"يااز";"ياانكه";"يااينكه";"يابد";"يارب";"يافتن";"يعني";"يقينا";"يك";"يكايك";"يكبار";"يكباره";"يكجا";"يكجانبه";"يكجور";"يكجوري";"يكدم";"يكديگر";"يكريز";"يكزمان";"يكسال";"يكسره";"يكسري";"يكطرفه";"يكنواخت";"يكي";"يكپارچه";"يه";"يواش";"پ";"پارسال";"پارسايانه";"پاره";"پايين";"پدرانه";"پدرپي";"پديده";"پذير";"پذيرند";"پراكنده";"پرتحرك";"پرخاشگرانه";"پرسان";"پرشتاب";"پرشور";"پروردگارا";"پريروز";"پس";"پشت";"پشتوانه";"پشيموني";"پنهان";"پهن";"پي";"پيامبرانه";"پيداست";"پيدرپي";"پيرامون";"پيش";"پيشاپيش";"پيشتر";"پيوسته";"پيگير";"چ";"چارده";"چاله";"چاپلوسانه";"چت";"چته";"چرا";"چراكه";"چطور";"چقدر";"چكار";"چنان";"چنانكه";"چنانچه";"چند";"چندان";"چنداني";"چندروزه";"چندماهه";"چندمين";"چنده";"چندين";"چنين";"چه";"چهارهزار";"چو";"چون";"چي";"چيز";"چيزهاست";"چيزيست";"چيست";"چيه";"چگونه";"ژ";"گ";"گاه";"گاهي";"گذاران";"گذاري";"گذاشتن";"گر";"گرديد";"گرفتارند";"گرفتن";"گرمي";"گرنه";"گرچه";"گفت";"گمان";"گهگاه";"گونه";"گويا";"گويان";"گويي";"ابلهانه ها"|]
    let RemoveStopWords arr =
        arr |> Seq.where (fun x -> (not (PersianStopWords.Contains x)) && x.Length > 2 && (IsLetterString x))

    let CleanseString = Hazm.Normalizer.Run >> Hazm.Stemmer.Stem

    let Trim (s:string) = s.TrimStart([|' '|]).TrimEnd([|' '|]).Replace("ـ", "")

    let DecodeAndCleanse = WebUtility.HtmlDecode >> CleanseString

    let RemoveTitleAnotAndCleanse = RemoveTitleAnnotation >> CleanseString

    let CleanseUserInput (str:string) = str |> Hazm.WordTokenizer.Tokenize |> Seq.map CleanseString |> RemoveStopWords |> Seq.map Trim

    let CleanseItem agency (jsonItem:NewsItem)  =
        let f = jsonItem
        {
            NewsAgency = agency
            Category = f.Category
            Title = f.Title |> Hazm.WordTokenizer.Tokenize |> Seq.map RemoveTitleAnotAndCleanse |> RemoveStopWords |> Seq.map Trim
            RuTitr  = f.RuTitr |> Hazm.WordTokenizer.Tokenize |> Seq.map CleanseString |> RemoveStopWords |> Seq.map Trim
            Subtitle = f.Subtitle |> Hazm.WordTokenizer.Tokenize |> Seq.map CleanseString |> RemoveStopWords |> Seq.map Trim
            Body = f.Body |> Hazm.WordTokenizer.Tokenize |> Seq.map DecodeAndCleanse |> RemoveStopWords |> Seq.map Trim
            Tags = f.Tags
        }

    let GetCategoryFromString str =
        match str with
        | "اجتماعی" -> AsrIranCategories.Ejtemaee
        | "سیاسی" -> AsrIranCategories.Siasi
        | "ورزشی" -> AsrIranCategories.Varzeshi
        | "بین الملل" -> AsrIranCategories.Beinolmelal
        | "فرهنگی/هنری" -> AsrIranCategories.FarhangiHonari
        | "اقتصادی" -> AsrIranCategories.Eghtesadi
        | "سیاست خارجی" -> AsrIranCategories.SiasatKhareji
        | "خواندنی ها و دیدنی ها" -> AsrIranCategories.KhandaniVaDidani
        | "عمومی" -> AsrIranCategories.Omoomi
        | "سلامت" -> AsrIranCategories.Salamat
        | "علمی" -> AsrIranCategories.Elmi
        | "روانشناسی" -> AsrIranCategories.Ravanshenasi
        | "حوادث" -> AsrIranCategories.Havades
        | "سرگرمی" -> AsrIranCategories.Sargarmi
        | "فناوری و IT" -> AsrIranCategories.IT
        | "داستان کوتاه" -> AsrIranCategories.DastanKootah
        | "دانلود" -> AsrIranCategories.Download
        | "آشپزی" -> AsrIranCategories.Ashpazi
        | "پیامک" -> AsrIranCategories.Payamak
        | "سفر و تفریح" -> AsrIranCategories.SafarOTafrih
        | _ -> AsrIranCategories.UNKNOWN

    let GetDocFromItem (item : CleansedItem) =
        let a = item
        Document (Seq.concat (seq {
                    yield a.Title
                    yield a.Subtitle
                    yield a.RuTitr
                    yield a.Body
                    yield a.Tags
                    }), (Seq.head item.Category) |> GetCategoryFromString)

    let GetFormalFormFromCleansedItem (item:CleansedItem) =
        let a = item
        FormalDocument (Seq.concat (seq {
                    yield a.Title
                    yield a.Subtitle
                    yield a.RuTitr
                    yield a.Body
                    yield a.Tags
                   }), NewsAgency.FarsNews)

    let GetFormalFormAsrDoc (item : AsrDocument) =
        let a = item
        match a with
            | Document doc ->
                let (words, _) = doc
                FormalDocument (words, NewsAgency.AsrIran)

module DataLoad =
    open JsonLinesParser
    open DataCleanser
    open Newtonsoft.Json

    let CategoryDetectorPath =
        Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Data/A_CategoryDetector/AsrIran.jsonl")

    let AgencyDetectorPath =
        Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Data/B_AgencyDetector/FarsNews.jsonl")

    let FetchJsonData (path:string) =
            File.ReadAllText path |> ToLines |> Seq.map (JsonConvert.DeserializeObject<NewsItem>)

    let FetchCleansedData path agency = FetchJsonData path |> Seq.map (CleanseItem agency)

    let FetchAsrDocuments = FetchCleansedData CategoryDetectorPath NewsAgency.AsrIran |> Seq.map GetDocFromItem
    let FetchFarsCleansedItems = FetchCleansedData AgencyDetectorPath NewsAgency.FarsNews

    let FetchAsrFormalForm = FetchAsrDocuments |> Seq.map GetFormalFormAsrDoc
    let FetchFarsFormalForm = FetchFarsCleansedItems |> Seq.map GetFormalFormFromCleansedItem

    let AsrTrainSet (set : AsrDocument seq) = Seq.take 9000 set
    let AsrTestSet  (set : AsrDocument seq) = Seq.skip 9000 set

    let FormalTrainSet (set : FormalDocument seq) = Seq.take 9000 set
    let FormalTestSet  (set : FormalDocument seq) = Seq.skip 9000 set

module DataProducer =
    open DataLoad
    open TfIdf
    open System.IO
    open Newtonsoft.Json

    let CreateTfIdfJsons () =
        let data = AsrTrainSet FetchAsrDocuments
        let mutable i = 1
        for doc in data do
            printfn "Processing document %d (%d %%)" i (i /90)
            i <- i + 1

            match doc with
            | Document d ->
                let (wordList, cat) = d
                for word in wordList do
                    if TfStore.ContainsKey(cat) then
                        if TfStore.[cat].ContainsKey(word) then
                            TfStore.[cat].[word] <- TfStore.[cat].[word] + 1
                        else
                            TfStore.[cat].[word] <- 1
                    else TfStore.[cat] <- new Dictionary<string, int>()

                    if DfStore.ContainsKey(word) then DfStore.[word].Add(doc) |> ignore
                    else DfStore.[word] <- new HashSet<AsrDocument>()

        printfn "Writing TF file..."
        let dfSave = new Dictionary<string, int>()
        for item in DfStore do
            dfSave.Add(item.Key, item.Value.Count)

        let tfJson = JsonConvert.SerializeObject(TfStore)
        File.WriteAllText (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Data/TfIdf/tf.json"), tfJson)

        printfn "Writing idf file..."
        let dfJson = JsonConvert.SerializeObject(dfSave)
        File.WriteAllText (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/TfIdf/df.json"), dfJson)
        ()

module AgencyDataProducer =
    open DataLoad
    open AgencyTfIdf
    open System.IO
    open Newtonsoft.Json

    let CreateAgencyTfIdfJsons () =
        let data1 = FormalTrainSet FetchAsrFormalForm
        let data2 = FormalTrainSet FetchFarsFormalForm
        let data = Seq.concat (seq{
                                yield data2
                                yield data1})
        let mutable i = 1
        for doc in data do
            printfn "Processing document %d (%d %%)" i (i /180)
            i <- i + 1

            match doc with
            | FormalDocument d ->
                let (wordList, cat) = d
                for word in wordList do
                    if TfStore.ContainsKey(cat) then
                        if TfStore.[cat].ContainsKey(word) then
                            TfStore.[cat].[word] <- TfStore.[cat].[word] + 1
                        else
                            TfStore.[cat].[word] <- 1
                    else TfStore.[cat] <- new Dictionary<string, int>()

                    if DfStore.ContainsKey(word) then DfStore.[word].Add(doc) |> ignore
                    else DfStore.[word] <- new HashSet<FormalDocument>()

        printfn "Writing TF file..."
        let dfSave = new Dictionary<string, int>()
        for item in DfStore do
            dfSave.Add(item.Key, item.Value.Count)

        let tfJson = JsonConvert.SerializeObject(TfStore)
        File.WriteAllText (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Data/AgencyTfIdf/tf.json"), tfJson)

        printfn "Writing idf file..."
        let dfJson = JsonConvert.SerializeObject(dfSave)
        File.WriteAllText (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/AgencyTfIdf/df.json"), dfJson)
        ()