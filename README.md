# ToDoListProjesi 📝 (Yapılacaklar Listesi Projesi)

Bu proje, **.NET Framework 4.7.2** üzerinde **C#** ve **Windows Forms** kullanılarak geliştirilmiş, **PostgreSQL** veritabanı ile etkileşim kuran basit bir Yapılacaklar Listesi (To-Do List) uygulamasıdır. Veritabanı işlemleri için **ADO.NET** ve **Npgsql** (PostgreSQL için .NET Veri Sağlayıcısı) kütüphanesi doğrudan kullanılmıştır.

## 🚀 Genel Bakış

Uygulama, kullanıcıların görevlerini (ToDo items) kategorilere ayırarak kaydetmelerine, düzenlemelerine, silmelerine ve listelemelerine olanak tanır. Görevler; başlık, açıklama, durum (tamamlandı/devam ediyor), öncelik ve kategori bilgileriyle yönetilir. Kategoriler de ayrı bir form üzerinden yönetilebilir. Bu proje, ADO.NET kullanarak PostgreSQL veritabanı ile nasıl temel CRUD (Create, Read, Update, Delete) işlemleri yapılacağını ve WinForms arayüzünün nasıl tasarlanacağını gösteren pratik bir örnektir.

## ✨ Özellikler

*   **Kategori Yönetimi (`FrmCategory`):**
    *   Yeni kategori ekleme.
    *   Mevcut kategorileri listeleme (`DataGridView`).
    *   Kategori silme.
    *   Kategori güncelleme.
    *   ID'ye göre kategori getirme ve listeleme.
*   **Yapılacaklar (To-Do) Yönetimi (`Form1`):**
    *   Yeni görev ekleme (Başlık, Açıklama, Öncelik, Kategori).
    *   Mevcut görevleri listeleme (`DataGridView`).
    *   Görev silme.
    *   Görev güncelleme.
    *   Görevleri kategori bilgisiyle birlikte listeleme (SQL `INNER JOIN`).
    *   Görevleri durumuna göre filtreleme (Tamamlananlar / Devam Edenler).
    *   Kategorileri `ComboBox` içinde listeleyerek görev ekleme/güncelleme sırasında seçim yapma.
*   **Veritabanı Erişimi:**
    *   **Npgsql:** PostgreSQL veritabanına bağlanmak ve sorgu çalıştırmak için kullanılır.
    *   **ADO.NET:** `NpgsqlConnection`, `NpgsqlCommand`, `NpgsqlDataAdapter`, `DataTable` gibi temel ADO.NET nesneleriyle doğrudan veritabanı işlemleri yapılır.
    *   **Parametreli Sorgular:** SQL Injection saldırılarını önlemek için `Parameters.AddWithValue` kullanılır.

## 🛠️ Kullanılan Teknolojiler

*   **Programlama Dili:** C#
*   **Framework:** .NET Framework 4.7.2
*   **Arayüz:** Windows Forms (WinForms)
*   **Veri Erişimi:** ADO.NET, Npgsql (v8.0.3 veya uyumlu)
*   **Veritabanı:** PostgreSQL

## 💾 Veritabanı Kurulumu (PostgreSQL)

Bu uygulamanın çalışması için yerel makinenizde veya erişilebilir bir sunucuda PostgreSQL veritabanının kurulu olması ve ilgili veritabanı ile tabloların oluşturulması gerekmektedir.

**1. PostgreSQL Kurulumu (Eğer kurulu değilse):**

*   **İndirme:** PostgreSQL'in resmi web sitesinden ([https://www.postgresql.org/download/](https://www.postgresql.org/download/)) işletim sisteminize uygun yükleyiciyi indirin.
*   **Kurulum:** Yükleyiciyi çalıştırın ve kurulum adımlarını takip edin.
    *   Kurulum sırasında genellikle veritabanı süper kullanıcısı (`postgres`) için bir **parola** belirlemeniz istenir. Bu parolayı unutmayın, bağlantı dizesinde kullanacaksınız.
    *   Varsayılan port genellikle **5432**'dir. Farklı bir port seçerseniz bağlantı dizesini güncellemeniz gerekir.
    *   Kurulumla birlikte genellikle **pgAdmin** gibi bir yönetim aracı da yüklenir. Bu aracı veritabanını oluşturmak ve yönetmek için kullanabilirsiniz.
*   **Servis Kontrolü:** Kurulumdan sonra PostgreSQL servisinin çalıştığından emin olun (Windows Hizmetleri'nden kontrol edilebilir).

**2. Veritabanı Oluşturma:**

*   **pgAdmin** veya başka bir PostgreSQL istemci aracı (psql komut satırı gibi) kullanarak sunucunuza bağlanın.
*   Aşağıdaki SQL komutunu çalıştırarak `ToDoListProjesi` adında yeni bir veritabanı oluşturun:
    ```sql
    CREATE DATABASE "ToDoListProjesi";
    ```
    *(Veritabanı adını farklı seçerseniz bağlantı dizesini de güncellemeniz gerekir.)*

**3. Tabloları Oluşturma:**

*   Oluşturduğunuz `ToDoListProjesi` veritabanını seçin ve yeni bir sorgu penceresi açın.
*   Proje içerisindeki `Queries/TextFile1.txt` dosyasında bulunan veya aşağıdaki SQL script'ini çalıştırarak `Categories` ve `ToDoLists` tablolarını oluşturun:

    ```sql
    CREATE TABLE IF NOT EXISTS public."Categories" -- IF NOT EXISTS eklemek tekrar çalıştırıldığında hata vermesini önler.
    (
        "CategoryId" integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ), -- PostgreSQL için serial yerine IDENTITY kullanımı daha modern.
        "CategoryName" character varying(100) COLLATE pg_catalog."default" NOT NULL,
        CONSTRAINT "Categories_pkey" PRIMARY KEY ("CategoryId")
    );

    CREATE TABLE IF NOT EXISTS public."ToDoLists" -- Tablo adının küçük harf olması PostgreSQL'de daha yaygındır ve sorunları önleyebilir.
    (
        todolistid integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
        title character varying(200) COLLATE pg_catalog."default" NOT NULL,
        description character varying(1000) COLLATE pg_catalog."default",
        status boolean, -- PostgreSQL'de BIT yerine BOOLEAN daha standarttır.
        priority character varying(50) COLLATE pg_catalog."default",
        categoryid integer,
        CONSTRAINT "ToDoLists_pkey" PRIMARY KEY (todolistid),
        CONSTRAINT todolists_categoryid_fkey FOREIGN KEY (categoryid)
            REFERENCES public."Categories" ("CategoryId") MATCH SIMPLE
            ON UPDATE NO ACTION
            ON DELETE CASCADE -- Kategori silindiğinde ilişkili görevler de silinsin. İsteğe bağlı.
    );

    -- PostgreSQL'de tablo ve sütun adlarının küçük harfle oluşturulması ve çift tırnak olmadan erişilmesi daha yaygındır.
    -- Eğer script'i bu şekilde çalıştırdıysanız, C# kodundaki sorgularda da küçük harf kullanmanız gerekebilir:
    -- Örnek C# sorgusu: "Select * From categories order by categoryid"
    -- Örnek C# sorgusu: "insert into todolists (title,description,status,priority,categoryid) values (@title,@description,true,@priority,@categoryid)"
    ```
    *   **Önemli Not:** PostgreSQL'de tablo ve sütun adları varsayılan olarak küçük harfe çevrilir. Eğer yukarıdaki gibi çift tırnak içinde büyük harf kullanarak (`"ToDoLists"`, `"CategoryId"`) oluşturduysanız, C# kodundaki sorgularda da bu çift tırnakları kullanmanız *gerekebilir*. Genellikle PostgreSQL'de her şeyi küçük harf yapmak daha sorunsuz bir deneyim sunar. Eğer kodunuzda `ToDoLists` veya `Categories` gibi büyük harfle başlayan isimler varsa ve sorun yaşarsanız, tablo/sütun adlarını SQL'de ve kodda küçük harfe çevirmeyi düşünebilirsiniz (`todolistid`, `todolists`, `categoryid`, `categories` vb.).

## ⚙️ Uygulama Yapılandırması (Connection String)

Uygulamanın PostgreSQL veritabanına bağlanabilmesi için bağlantı dizesinin doğru şekilde ayarlanması gerekir.

*   `ToDoListProjesi` projesindeki `Form1.cs` ve `FrmCategory.cs` dosyalarını açın.
*   Her iki dosyada da bulunan `connectionString` değişkenini bulun:
    ```csharp
    string connectionString = "Server=localHost;port=5432;Database=ToDoListProjesi;user ID=postgres;Password=1234";
    ```
*   Bu dizedeki değerleri kendi PostgreSQL kurulumunuza göre düzenleyin:
    *   `Server=localHost`: PostgreSQL sunucunuzun adresi (genellikle `localhost` veya IP adresi).
    *   `port=5432`: PostgreSQL sunucunuzun port numarası (varsayılan 5432).
    *   `Database=ToDoListProjesi`: Oluşturduğunuz veritabanının adı.
    *   `user ID=postgres`: PostgreSQL kullanıcı adınız (varsayılan `postgres`).
    *   `Password=1234`: Kurulum sırasında belirlediğiniz `postgres` kullanıcısının parolası.
*   **(Öneri):** Bağlantı dizesini kod içinde hard-code yazmak yerine projenin `App.config` dosyasına taşımak daha iyi bir pratiktir. Bu sayede şifre gibi hassas bilgiler kod içinde olmaz ve değiştirilmesi daha kolay olur.

## 🏃 Nasıl Çalıştırılır?

1.  **Gereksinimler:**
    *   Visual Studio 2019 veya üzeri (.NET Framework 4.7.2 desteği ile)
    *   Kurulu ve çalışan bir PostgreSQL sunucusu.
    *   Yukarıdaki adımlarla oluşturulmuş veritabanı ve tablolar.
2.  **Projeyi Klonlama:**
    ```bash
    git clone https://github.com/kullanici-adiniz/ToDoListProjesi.git
    ```
    *(kullanici-adiniz kısmını kendi GitHub kullanıcı adınızla değiştirin)*
3.  Yukarıdaki "Uygulama Yapılandırması" adımını takip ederek `Form1.cs` ve `FrmCategory.cs` içindeki bağlantı dizelerini güncelleyin.
4.  `ToDoListProjesi.sln` dosyasını Visual Studio ile açın.
5.  NuGet Paket Yöneticisi'nin Npgsql paketini geri yüklediğinden emin olun (`Tools -> NuGet Package Manager -> Manage NuGet Packages for Solution...` altından kontrol edilebilir veya proje derlendiğinde otomatik yapılır). Gerekirse `Update-Package -reinstall Npgsql` komutunu Paket Yöneticisi Konsolu'nda çalıştırın.
6.  Projeyi derleyin (Build -> Build Solution).
7.  Uygulamayı başlatın (Debug -> Start Debugging veya F5). `Form1` (ana To-Do listesi formu) açılacaktır.

## 🏗️ Proje Yapısı

*   **`/` (Kök Dizin):** Form sınıfları (`Form1.cs`, `FrmCategory.cs`), Program.cs ve proje/çözüm dosyaları.
*   **`/Queries`**: Veritabanı tablolarını oluşturmak için kullanılan SQL script'ini içerir.
*   **`/bin`**: Derlenmiş uygulama dosyaları.
*   **`/obj`**: Derleme sırasında oluşan geçici dosyalar.
*   **`/Properties`**: Assembly bilgileri, kaynaklar ve ayarlar.
