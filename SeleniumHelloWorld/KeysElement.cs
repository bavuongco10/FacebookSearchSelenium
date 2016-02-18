using System;
using System.Collections;

namespace SeleniumHelloWorld
{
    public sealed class KeysElement
    {
        private static readonly Lazy<KeysElement> lazy = new Lazy<KeysElement>(() => new KeysElement());

        public Hashtable Address = new Hashtable();
        public Hashtable Birthday = new Hashtable();
        public Hashtable CurrentCity = new Hashtable();
        public Hashtable Email = new Hashtable();
        public Hashtable Gender = new Hashtable();
        public Hashtable Hometown = new Hashtable();
        public Hashtable Interested = new Hashtable();
        public Hashtable Languages = new Hashtable();
        public Hashtable Mobile = new Hashtable();
        public Hashtable Political = new Hashtable();
        public Hashtable Relationship = new Hashtable();
        public Hashtable Religious = new Hashtable();
        public Hashtable Screennames = new Hashtable();
        public Hashtable Skills = new Hashtable();
        public Hashtable Website = new Hashtable();

        private KeysElement()
        {
            // For English page
            Mobile.Add(PageLanguage.en, "Mobile");
            Address.Add(PageLanguage.en, "Address");
            Screennames.Add(PageLanguage.en, "Screennames");
            Website.Add(PageLanguage.en, "Website");
            Email.Add(PageLanguage.en, "Email");
            Birthday.Add(PageLanguage.en, "Birthday");
            Gender.Add(PageLanguage.en, "Gender");
            Interested.Add(PageLanguage.en, "Interested");
            Languages.Add(PageLanguage.en, "Languages");
            Religious.Add(PageLanguage.en, "Religious");
            Political.Add(PageLanguage.en, "Political");
            Skills.Add(PageLanguage.en, "Skills");
            Relationship.Add(PageLanguage.en, "Relationship");
            CurrentCity.Add(PageLanguage.en, "Current");
            Hometown.Add(PageLanguage.en, "Hometown");

            // Tiếng việt page
            Mobile.Add(PageLanguage.vi, "Di động");
            Address.Add(PageLanguage.vi, "Địa chỉ");
            Screennames.Add(PageLanguage.vi, "Tên hiển thị");
            Website.Add(PageLanguage.vi, "Trang web");
            Email.Add(PageLanguage.vi, "Email");
            Birthday.Add(PageLanguage.vi, "Ngày sinh");
            Gender.Add(PageLanguage.vi, "Giới tính");
            Interested.Add(PageLanguage.vi, "Thích");
            Languages.Add(PageLanguage.vi, "Ngôn ngữ");
            Religious.Add(PageLanguage.vi, "Quan điểm tôn giáo");
            Political.Add(PageLanguage.vi, "Quan Điểm Chính Trị");
            Skills.Add(PageLanguage.vi, "Skills");
            Relationship.Add(PageLanguage.vi, "Relationship");
            CurrentCity.Add(PageLanguage.vi, "Current");
            Hometown.Add(PageLanguage.vi, "Hometown");
        }

        public static KeysElement Instance
        {
            get { return lazy.Value; }
        }
    }
}