# Test Matrix для Lab 36

Цей документ показує відповідність між use cases, ризиками та тестами. Назви тестів наведені за поточним кодом у `tests/CarRentSystemstem.Tests`.

## Покриття use cases

| Use case | Ризик | Тести |
| --- | --- | --- |
| UC-1. Оренда доступного авто | авто не змінює стан, rental не створюється | `RentalServiceTests.Should_Rent_Car`, `FullFlowIntegrationTests.Full_Rent_And_Return_Flow_Should_Work`, `RentalSystemIntegrationTests.Should_Rent_After_Reload` |
| UC-2. Повторна оренда зайнятого авто | неконсистентний стан і duplicate rentals | `CarTests.ShouldNotAllowDoubleRent`, `RentalServiceErrorHandlingTests.Should_Throw_CarAlreadyRentedException_When_Car_Not_Available`, `StateTransitionTests.Car_Should_Not_Allow_Double_Rent` |
| UC-3. Повернення авто | авто не стає доступним після return | `RentalServiceTests.Should_Return_Car`, `FullFlowIntegrationTests.Full_Rent_And_Return_Flow_Should_Work`, `RentalSystemIntegrationTests.Should_Return_After_Restore` |
| UC-4. Повернення неіснуючого авто | відсутній domain exception | `RentalServiceErrorHandlingTests.Should_Throw_RentalNotFoundException_When_No_Active_Rental`, `FullFlowIntegrationTests.Returning_Invalid_Car_Should_Throw` |
| UC-5. Невалідний тип клієнта | неправильна знижка або неочікуваний стан | `RentalServiceErrorHandlingTests.Should_Throw_InvalidCustomerTypeException_For_Invalid_Customer_Type`, `RentalServiceErrorHandlingTests.All_Domain_Exceptions_Should_Have_Error_Context`, `TheoryTests.Customer_Should_Reject_Invalid_Name` |
| UC-6. Ліміти оренди | economy/premium rules порушені | `RentalServiceErrorHandlingTests.Should_Throw_RentalLimitExceededException_For_Economy_Over_10_Days`, `BoundaryTests.Should_Reject_Too_Long_Rental`, `TheoryTests.Rental_Should_Reject_Invalid_Days` |
| UC-7. Розрахунок ціни | неправильний total price | `RentalRulesTests.Rental_Should_Calculate_Price`, `ExtendedDomainTests.Rental_Should_Calculate_Price_With_Economy_Discount`, `ExtendedDomainTests.Rental_Should_Calculate_Price_With_Premium_Discount` |
| UC-8. Ціна моделі авто | старий hotspot у `Car.GetDefaultPrice()` | `ExtendedDomainTests.CarPricingConfiguration_Should_Get_BMW_X5_Price`, `ExtendedDomainTests.CarPricingConfiguration_Should_Get_Default_Price_For_Unknown_Model`, `ExtendedDomainTests.CarPricingConfiguration_All_Models_Should_Have_Valid_Prices` |
| UC-9. Money value object | від'ємні суми, неправильні порівняння | `MoneyTests.Constructor_Should_Reject_Negative_Amount`, `MoneyTests.Addition_Should_Return_New_Money_With_Summed_Amount`, `MoneyTests.GreaterThan_Should_Compare_Amounts`, `MoneyTests.LessThan_Should_Compare_Amounts` |
| UC-10. Facade для сценаріїв застосунку | UI/console залежить від внутрішніх деталей | `FacadeTests.Facade_Should_Rent_Car`, `FacadeTests.Facade_Should_Return_Car`, `FacadeTests.Facade_Should_Calculate_Revenue_After_Rental` |
| UC-11. Аналітика оренд | LINQ падає на порожніх даних або рахує неправильно | `RentalAnalyticsServiceTests.GetTotalRevenue_Should_Return_Zero_For_No_Rentals`, `RentalAnalyticsServiceTests.GetCarPopularity_Should_Count_Rentals_Per_Model`, `RentalAnalyticsServiceTests.GetTopRentedCars_Should_Order_By_Rental_Count_And_Limit_To_Five`, `RentalAnalyticsServiceTests.GetRentalReport_Should_Use_Filter_And_Linq_Aggregations`, `RentalAnalyticsServiceTests.GetCachedRentalReport_Should_Reuse_Report_Until_Cache_Is_Cleared` |
| UC-12. Пошук клієнта | case-sensitive або неправильна фільтрація | `RentalAnalyticsServiceTests.SearchByCustomer_Should_Search_Case_Insensitively` |
| UC-13. Збереження у файл | дані не записуються або втрачаються | `PersistenceIntegrationTests.Save_And_Load_Should_Preserve_Data`, `FileStorageTests.SaveAsync_And_LoadAsync_Should_RoundTrip_Cars`, `JsonDataStoreResultTests.Save_Should_Write_File_And_Return_Success` |
| UC-14. Повторне завантаження | стан після restore неправильний | `PersistenceIntegrationTests.Loaded_Car_Should_Keep_Model`, `RentalSystemIntegrationTests.Should_Rent_After_Reload`, `RentalSystemIntegrationTests.Full_System_Flow_Should_Work` |
| UC-15. Missing file | crash при першому запуску | `PersistenceIntegrationTests.Missing_File_Should_Return_Empty_List`, `PersistenceErrorHandlingTests.JsonDataStore_Load_Should_Return_Empty_List_When_File_Missing`, `FileStorageTests.LoadAsync_Should_Return_Empty_List_When_File_Does_Not_Exist` |
| UC-16. Corrupted JSON | застосунок падає на пошкодженому файлі | `PersistenceErrorHandlingTests.JsonDataStore_Load_Should_Handle_Corrupted_Json_Gracefully`, `FileStorageTests.LoadAsync_Should_Return_Empty_List_For_Corrupted_Json`, `JsonServiceTests.Deserialize_Should_Throw_For_Invalid_Json` |
| UC-17. I/O failure | помилка не повертає контрольований результат | `PersistenceErrorHandlingTests.JsonDataStore_Should_Return_Fail_On_Invalid_Path_For_Save`, `JsonDataStoreResultTests.LoadResult_Should_Return_Failure_For_Invalid_Json` |
| UC-18. Логування подій | fault scenario лишається невидимим | `ApplicationEventBusTests.Notify_Should_Pass_Message_To_Subscribed_Listeners`, `PersistenceErrorHandlingTests.ApplicationEventBus_Should_Log_Persistence_Errors`, `ConsoleLoggerTests.Handle_Should_Write_Event_Message` |
| UC-19. DTO/Result wrappers | edge cases не покриті | `RentalDtoTests.RentalDto_Should_Store_CarModel_And_CustomerName`, `ResultTests.Ok_Should_Create_Success_Result`, `ResultTests.Fail_Should_Create_Failure_Result`, `GenericResultTests.Fail_Should_Create_Failure_Result_Without_Value` |

## Мінімальні вимоги Lab 36

| Вимога | Статус | Доказ |
| --- | --- | --- |
| Мінімум 20 unit-тестів | Виконано | Понад 160 unit test methods, 220 xUnit cases total |
| Мінімум 8 integration-тестів | Виконано | 25 integration test methods у `tests/CarRentSystemstem.Tests/Integration` |
| Негативні сценарії | Виконано | invalid car, duplicate rent, invalid customer, corrupted JSON, missing file, invalid path |
| Theory tests | Виконано | `TheoryTests`, `MoneyTests`, `RentalAnalyticsServiceTests`, `ResultTests`, `EventListenerTests` |
| Builders/fixtures | Виконано | `TestDataFixture`, `TestPathHelper`, in-memory repositories |
| Coverage report | Виконано | `coverage-report/index.html` |
| CI quality gate | Виконано | `.github/workflows/dotnet.yml` |

## Компоненти з попереднього 0% coverage

| Компонент | Поточний статус |
| --- | --- |
| `RentalAnalyticsService` | Покрито unit-тестами |
| `ConsoleLogger` | Покрито unit-тестами з `Console.Out` capture |
| `RentalEventListener` | Покрито unit-тестами |
| `RentalDto` | Покрито unit-тестом |
| `Result` / `Result<T>` | Покрито success/failure/edge cases |
| `Money` | Покрито constructor, negative value, addition, comparison |
| `FileStorage` | Покрито save/load/missing/corrupted/not-opened cases |
| `JsonService` | Покрито serialize/deserialize/invalid JSON |

## Підсумок

Поточний набір тестів покриває core domain, application services, persistence, analytics, event logging і fault handling. В HTML-звіті немає класів з 0% executable line coverage.
