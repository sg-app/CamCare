# [1.1.0-beta.1](https://github.com/sg-app/CamCare/compare/v1.0.1-beta.1...v1.1.0-beta.1) (2026-01-15)


### Features

* **forms:** migrate to Blazilla, improve validation UX ([8cd87c8](https://github.com/sg-app/CamCare/commit/8cd87c850ac0d8b98dffc859b84829f3f787490e))
* **repair-order:** add packaging type and dimensions ([dd6aaf1](https://github.com/sg-app/CamCare/commit/dd6aaf1e00511039188244ca83e8649081473d16))

## [1.0.1-beta.1](https://github.com/sg-app/CamCare/compare/v1.0.0...v1.0.1-beta.1) (2025-12-10)


### Bug Fixes

* remove debounce from OnDescriptionChanged methods ([c70b9d7](https://github.com/sg-app/CamCare/commit/c70b9d7647b38dda16fd56ba70eaa00dbe0b1bcd))

# 1.0.0 (2025-11-20)


### Bug Fixes

* comment out automatic database migration ([3e7c4e2](https://github.com/sg-app/CamCare/commit/3e7c4e233bad9e51d50424fa0a1c19240aef1e8e))
* **database:** replace EnsureCreatedAsync with MigrateAsync ([6fd360a](https://github.com/sg-app/CamCare/commit/6fd360a61881cda5dcee0ff401738e2e26fb46f7))
* dbcreation and update .gitignore for database files ([4eeac0e](https://github.com/sg-app/CamCare/commit/4eeac0e56c115a7f3adee14a279ad8f3f1ae1ec3))
* improve formatting and structure in RepairOrderPage ([b4d39c7](https://github.com/sg-app/CamCare/commit/b4d39c72dd4ed735845c6e7ad3d169fb8253db9f))
* **persistence:** improve connection string handling ([870b48d](https://github.com/sg-app/CamCare/commit/870b48daa3edb2a0e1b89d162bf680642b1d5064))
* **persistence:** simplify SQLite connection string retrieval ([b6113bf](https://github.com/sg-app/CamCare/commit/b6113bf92990ad1953ae62446ab3098f64af9f68))
* **persistence:** update SQLite database connection path ([5f68c28](https://github.com/sg-app/CamCare/commit/5f68c28bb0e2a669d0382fe1c46c17e0a1e6af24))
* update .gitignore for MigrationBackup and CamCare ([86be1b5](https://github.com/sg-app/CamCare/commit/86be1b534419b3b62ddccae02f2971f7c6a84c1b))
* update AddPersistence method to use configuration ([9992a30](https://github.com/sg-app/CamCare/commit/9992a3053290d5b4e6ad28016af7583aef4bc17d))


### Features

* add `RadzenDataGrid` for managing `Artikel` data ([25a0080](https://github.com/sg-app/CamCare/commit/25a0080a52e35f74a0f065b824c66132db915a2d))
* add archived orders filter and update data retrieval ([7a59803](https://github.com/sg-app/CamCare/commit/7a598036004b3c7a1680101e46728f34b82ed579))
* add arrival date field and improve formatting ([1bc9bba](https://github.com/sg-app/CamCare/commit/1bc9bba5861cd447ad6a67faaa0cec33f85b31bd))
* add customer details display in CameraPage ([045cf18](https://github.com/sg-app/CamCare/commit/045cf1894d6372d740fa1ed1133ef589ce171e60))
* add DataStore entity and enhance delete behavior ([9a55178](https://github.com/sg-app/CamCare/commit/9a5517824a7fd924a61ef30ab282550c2c1b5104))
* Add employee management and update project structure ([36e7bbf](https://github.com/sg-app/CamCare/commit/36e7bbf31354f85676bd107a574fdda86397c43e))
* add file download support and improve file uploads ([1af1282](https://github.com/sg-app/CamCare/commit/1af1282bd9d7ef58cd5bc6f3a03f985dda80ec1c))
* add InsertButton and refactor BadageStyle handling ([82ae4d5](https://github.com/sg-app/CamCare/commit/82ae4d5c391fe3424c52d155bb7a234cf51953b6))
* add Radzen tiles for order management ([556da20](https://github.com/sg-app/CamCare/commit/556da20a33d47a9e8d02c50577099986756aa40d))
* Add support for managing Included Components ([8dc3107](https://github.com/sg-app/CamCare/commit/8dc31075632dfbe8b007f2b748a9ea138ef8ca9a))
* add validation to CreateCamCareEntryPage ([7dde586](https://github.com/sg-app/CamCare/commit/7dde586dbd11da2200b6b8f1d65b6bec65ad15f5))
* add video upload and preview support ([5a9055e](https://github.com/sg-app/CamCare/commit/5a9055ea0e4157a2cbf5173ac11cb1babfd70b8c))
* **data-grid:** enhance filtering in CameraPage ([a5efa73](https://github.com/sg-app/CamCare/commit/a5efa730a13a90366d2e6a95d867e6b826a97b62))
* **docker:** add data volume for persistent storage ([2afb2be](https://github.com/sg-app/CamCare/commit/2afb2be9486ce7bcd87e7e0e16e5be50fe21cc19))
* enhance camera assignment and UI logic ([582bb4e](https://github.com/sg-app/CamCare/commit/582bb4efb139d6347c302aeddb09ac6600480c51))
* Enhance camera management and UI components ([aafbab2](https://github.com/sg-app/CamCare/commit/aafbab26f16ccccd77ffdf32289c2a7b266acf46))
* enhance customer data retrieval and UI display ([0542be8](https://github.com/sg-app/CamCare/commit/0542be85117666689c8cbccda923d64168dd32a7))
* enhance customer editing and validation features ([486f232](https://github.com/sg-app/CamCare/commit/486f23233aecd4f570a5d0059f248ffcf30b43e0))
* Enhance customer management and logging ([4f77946](https://github.com/sg-app/CamCare/commit/4f7794645119356a218c1b12f9e8fcc9e8c9c8e0))
* enhance layout and structure of address components ([30657b3](https://github.com/sg-app/CamCare/commit/30657b32e3acffe19442157f126d5cd51cfa26ad))
* Enhance Razor components and add new features ([a356919](https://github.com/sg-app/CamCare/commit/a356919a46409ffcacd5a0220e53c27a0dcb1650))
* Enhance repair order management and UI components ([66f6b14](https://github.com/sg-app/CamCare/commit/66f6b14c3aac631a74f883eb74e79493dc9a8800))
* enhance Repair Order Status functionality ([d4b8472](https://github.com/sg-app/CamCare/commit/d4b84722c353bdf440603c2c38c3208cbbda4299))
* enhance repair order workflow and data management ([cf5a532](https://github.com/sg-app/CamCare/commit/cf5a5323315d5a2a8a5c96a93e38d6bbeb37065f))
* enhance routing and modify customer insertion logic ([7fd53f4](https://github.com/sg-app/CamCare/commit/7fd53f49bba1a925558315266720b7465c1fc175))
* Enhance validation and customer management features ([ba90e31](https://github.com/sg-app/CamCare/commit/ba90e31ce51b150c99a8d33c68954074d04fe0af))
* improve checkbox handling in RepairOrderEditorPage ([0bbd8f1](https://github.com/sg-app/CamCare/commit/0bbd8f11e0a1945a9369e6c65472c9681d4da6ea))
* integrate Amicron database for customer management ([96b22a4](https://github.com/sg-app/CamCare/commit/96b22a4060a25b149ced129677c0ef11aabcdd6a))
* Refactor CreateCamCareEntryPage and add logistic provider ([d59fd2d](https://github.com/sg-app/CamCare/commit/d59fd2d3851301b3beaf1248516dde3b8a4edd13))
* Refactor CreateCamCareEntryPage and update models ([bd792e4](https://github.com/sg-app/CamCare/commit/bd792e460e0f34873b505f7e2d482de8e56d1da9))
* remove Camera and CameraType entities ([e62abb2](https://github.com/sg-app/CamCare/commit/e62abb2905230f1726584900cd5029e86228c7b9))
* update column widths in KrdPage data grid ([1fbd9d0](https://github.com/sg-app/CamCare/commit/1fbd9d08c698b6d63828ce6bbf782563d81f0fe7))
* update Dockerfile and improve database handling ([9ee7d8a](https://github.com/sg-app/CamCare/commit/9ee7d8aa29252f3fbc074d4ceb9939c14d8f18be))
* update Dockerfile for CamCare project publishing ([283eac2](https://github.com/sg-app/CamCare/commit/283eac20b63f5ab6dde72d3cf1bc97063fb5976a))
* update project configuration and database context ([c01d0f3](https://github.com/sg-app/CamCare/commit/c01d0f3ed2fd141ec3b2758780f556c5d52903f1))
* Update project for .NET 8.0 and Docker support ([dba458b](https://github.com/sg-app/CamCare/commit/dba458b3d12f5c26139ebd007952e2df9686506d))
* Update repair order management and add Krd data ([ab993fa](https://github.com/sg-app/CamCare/commit/ab993fa923a7116a58c014798d2a3331f1395309))
* Update solution and enhance RepairOrderStatusPage ([da0fbf2](https://github.com/sg-app/CamCare/commit/da0fbf22e8e5d1d2e832b72df6365a411c46d3c6))
* update UI text and improve error logging ([238a58b](https://github.com/sg-app/CamCare/commit/238a58b72e5ebbd6d4117d50d403567023f7a90a))

# [1.0.0-beta.19](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.18...v1.0.0-beta.19) (2025-11-20)


### Features

* remove Camera and CameraType entities ([e62abb2](https://github.com/sg-app/CamCare/commit/e62abb2905230f1726584900cd5029e86228c7b9))

# [1.0.0-beta.18](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.17...v1.0.0-beta.18) (2025-11-19)


### Features

* add DataStore entity and enhance delete behavior ([9a55178](https://github.com/sg-app/CamCare/commit/9a5517824a7fd924a61ef30ab282550c2c1b5104))
* add file download support and improve file uploads ([1af1282](https://github.com/sg-app/CamCare/commit/1af1282bd9d7ef58cd5bc6f3a03f985dda80ec1c))
* add video upload and preview support ([5a9055e](https://github.com/sg-app/CamCare/commit/5a9055ea0e4157a2cbf5173ac11cb1babfd70b8c))
* enhance repair order workflow and data management ([cf5a532](https://github.com/sg-app/CamCare/commit/cf5a5323315d5a2a8a5c96a93e38d6bbeb37065f))

# [1.0.0-beta.17](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.16...v1.0.0-beta.17) (2025-11-16)


### Features

* Add support for managing Included Components ([8dc3107](https://github.com/sg-app/CamCare/commit/8dc31075632dfbe8b007f2b748a9ea138ef8ca9a))

# [1.0.0-beta.16](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.15...v1.0.0-beta.16) (2025-11-13)


### Features

* add `RadzenDataGrid` for managing `Artikel` data ([25a0080](https://github.com/sg-app/CamCare/commit/25a0080a52e35f74a0f065b824c66132db915a2d))
* enhance camera assignment and UI logic ([582bb4e](https://github.com/sg-app/CamCare/commit/582bb4efb139d6347c302aeddb09ac6600480c51))
* Enhance customer management and logging ([4f77946](https://github.com/sg-app/CamCare/commit/4f7794645119356a218c1b12f9e8fcc9e8c9c8e0))
* improve checkbox handling in RepairOrderEditorPage ([0bbd8f1](https://github.com/sg-app/CamCare/commit/0bbd8f11e0a1945a9369e6c65472c9681d4da6ea))
* integrate Amicron database for customer management ([96b22a4](https://github.com/sg-app/CamCare/commit/96b22a4060a25b149ced129677c0ef11aabcdd6a))

# [1.0.0-beta.15](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.14...v1.0.0-beta.15) (2025-08-13)


### Features

* add Radzen tiles for order management ([556da20](https://github.com/sg-app/CamCare/commit/556da20a33d47a9e8d02c50577099986756aa40d))
* enhance routing and modify customer insertion logic ([7fd53f4](https://github.com/sg-app/CamCare/commit/7fd53f49bba1a925558315266720b7465c1fc175))

# [1.0.0-beta.14](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.13...v1.0.0-beta.14) (2025-08-10)


### Features

* enhance customer editing and validation features ([486f232](https://github.com/sg-app/CamCare/commit/486f23233aecd4f570a5d0059f248ffcf30b43e0))
* update column widths in KrdPage data grid ([1fbd9d0](https://github.com/sg-app/CamCare/commit/1fbd9d08c698b6d63828ce6bbf782563d81f0fe7))

# [1.0.0-beta.13](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.12...v1.0.0-beta.13) (2025-08-09)


### Features

* Add employee management and update project structure ([36e7bbf](https://github.com/sg-app/CamCare/commit/36e7bbf31354f85676bd107a574fdda86397c43e))
* Enhance Razor components and add new features ([a356919](https://github.com/sg-app/CamCare/commit/a356919a46409ffcacd5a0220e53c27a0dcb1650))
* Update repair order management and add Krd data ([ab993fa](https://github.com/sg-app/CamCare/commit/ab993fa923a7116a58c014798d2a3331f1395309))

# [1.0.0-beta.12](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.11...v1.0.0-beta.12) (2025-08-07)


### Bug Fixes

* **database:** replace EnsureCreatedAsync with MigrateAsync ([6fd360a](https://github.com/sg-app/CamCare/commit/6fd360a61881cda5dcee0ff401738e2e26fb46f7))

# [1.0.0-beta.11](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.10...v1.0.0-beta.11) (2025-08-07)


### Bug Fixes

* comment out automatic database migration ([3e7c4e2](https://github.com/sg-app/CamCare/commit/3e7c4e233bad9e51d50424fa0a1c19240aef1e8e))

# [1.0.0-beta.10](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.9...v1.0.0-beta.10) (2025-08-07)


### Bug Fixes

* **persistence:** simplify SQLite connection string retrieval ([b6113bf](https://github.com/sg-app/CamCare/commit/b6113bf92990ad1953ae62446ab3098f64af9f68))

# [1.0.0-beta.9](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.8...v1.0.0-beta.9) (2025-08-07)


### Features

* **docker:** add data volume for persistent storage ([2afb2be](https://github.com/sg-app/CamCare/commit/2afb2be9486ce7bcd87e7e0e16e5be50fe21cc19))

# [1.0.0-beta.8](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.7...v1.0.0-beta.8) (2025-08-07)


### Features

* update Dockerfile for CamCare project publishing ([283eac2](https://github.com/sg-app/CamCare/commit/283eac20b63f5ab6dde72d3cf1bc97063fb5976a))

# [1.0.0-beta.7](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.6...v1.0.0-beta.7) (2025-08-07)


### Bug Fixes

* **persistence:** improve connection string handling ([870b48d](https://github.com/sg-app/CamCare/commit/870b48daa3edb2a0e1b89d162bf680642b1d5064))
* **persistence:** update SQLite database connection path ([5f68c28](https://github.com/sg-app/CamCare/commit/5f68c28bb0e2a669d0382fe1c46c17e0a1e6af24))

# [1.0.0-beta.6](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.5...v1.0.0-beta.6) (2025-08-07)


### Bug Fixes

* update .gitignore for MigrationBackup and CamCare ([86be1b5](https://github.com/sg-app/CamCare/commit/86be1b534419b3b62ddccae02f2971f7c6a84c1b))

# [1.0.0-beta.5](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.4...v1.0.0-beta.5) (2025-08-07)


### Bug Fixes

* dbcreation and update .gitignore for database files ([4eeac0e](https://github.com/sg-app/CamCare/commit/4eeac0e56c115a7f3adee14a279ad8f3f1ae1ec3))


### Features

* update Dockerfile and improve database handling ([9ee7d8a](https://github.com/sg-app/CamCare/commit/9ee7d8aa29252f3fbc074d4ceb9939c14d8f18be))

# [1.0.0-beta.4](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.3...v1.0.0-beta.4) (2025-08-07)


### Bug Fixes

* update AddPersistence method to use configuration ([9992a30](https://github.com/sg-app/CamCare/commit/9992a3053290d5b4e6ad28016af7583aef4bc17d))

# [1.0.0-beta.3](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.2...v1.0.0-beta.3) (2025-08-06)


### Features

* update UI text and improve error logging ([238a58b](https://github.com/sg-app/CamCare/commit/238a58b72e5ebbd6d4117d50d403567023f7a90a))

# [1.0.0-beta.2](https://github.com/sg-app/CamCare/compare/v1.0.0-beta.1...v1.0.0-beta.2) (2025-08-06)


### Features

* update project configuration and database context ([c01d0f3](https://github.com/sg-app/CamCare/commit/c01d0f3ed2fd141ec3b2758780f556c5d52903f1))

# 1.0.0-beta.1 (2025-08-05)


### Bug Fixes

* improve formatting and structure in RepairOrderPage ([b4d39c7](https://github.com/sg-app/CamCare/commit/b4d39c72dd4ed735845c6e7ad3d169fb8253db9f))


### Features

* add archived orders filter and update data retrieval ([7a59803](https://github.com/sg-app/CamCare/commit/7a598036004b3c7a1680101e46728f34b82ed579))
* add arrival date field and improve formatting ([1bc9bba](https://github.com/sg-app/CamCare/commit/1bc9bba5861cd447ad6a67faaa0cec33f85b31bd))
* add customer details display in CameraPage ([045cf18](https://github.com/sg-app/CamCare/commit/045cf1894d6372d740fa1ed1133ef589ce171e60))
* add InsertButton and refactor BadageStyle handling ([82ae4d5](https://github.com/sg-app/CamCare/commit/82ae4d5c391fe3424c52d155bb7a234cf51953b6))
* add validation to CreateCamCareEntryPage ([7dde586](https://github.com/sg-app/CamCare/commit/7dde586dbd11da2200b6b8f1d65b6bec65ad15f5))
* **data-grid:** enhance filtering in CameraPage ([a5efa73](https://github.com/sg-app/CamCare/commit/a5efa730a13a90366d2e6a95d867e6b826a97b62))
* Enhance camera management and UI components ([aafbab2](https://github.com/sg-app/CamCare/commit/aafbab26f16ccccd77ffdf32289c2a7b266acf46))
* enhance customer data retrieval and UI display ([0542be8](https://github.com/sg-app/CamCare/commit/0542be85117666689c8cbccda923d64168dd32a7))
* enhance layout and structure of address components ([30657b3](https://github.com/sg-app/CamCare/commit/30657b32e3acffe19442157f126d5cd51cfa26ad))
* Enhance repair order management and UI components ([66f6b14](https://github.com/sg-app/CamCare/commit/66f6b14c3aac631a74f883eb74e79493dc9a8800))
* enhance Repair Order Status functionality ([d4b8472](https://github.com/sg-app/CamCare/commit/d4b84722c353bdf440603c2c38c3208cbbda4299))
* Enhance validation and customer management features ([ba90e31](https://github.com/sg-app/CamCare/commit/ba90e31ce51b150c99a8d33c68954074d04fe0af))
* Refactor CreateCamCareEntryPage and add logistic provider ([d59fd2d](https://github.com/sg-app/CamCare/commit/d59fd2d3851301b3beaf1248516dde3b8a4edd13))
* Refactor CreateCamCareEntryPage and update models ([bd792e4](https://github.com/sg-app/CamCare/commit/bd792e460e0f34873b505f7e2d482de8e56d1da9))
* Update project for .NET 8.0 and Docker support ([dba458b](https://github.com/sg-app/CamCare/commit/dba458b3d12f5c26139ebd007952e2df9686506d))
* Update solution and enhance RepairOrderStatusPage ([da0fbf2](https://github.com/sg-app/CamCare/commit/da0fbf22e8e5d1d2e832b72df6365a411c46d3c6))
