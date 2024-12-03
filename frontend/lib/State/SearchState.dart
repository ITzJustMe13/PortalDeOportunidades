import 'package:flutter/material.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:http/http.dart' as http;

class SearchState with ChangeNotifier {
  var _opportunityApiHandler = OpportunityApiHandler();

  SearchState() {
    initializeOpportunitiesList();
  }

  set opportunityApiHandler(OpportunityApiHandler handler) {
    _opportunityApiHandler = handler;
  }

  List<Opportunity> _opportunitiesList = [];
  bool _isTopChecked = false;
  bool _isLoading = false;
  String? _selectedSort;
  bool _isDrawerOpen = false;

  String? _keyword;
  Location? _location;
  OppCategory? _category;
  int? _vacancies;
  double? _minPrice;
  double? _maxPrice;

  final TextEditingController _keywordController = TextEditingController();
  final TextEditingController _vacanciesController = TextEditingController();
  final TextEditingController _minPriceController = TextEditingController();
  final TextEditingController _maxPriceController = TextEditingController();

  void updateIsDrawerOpen() {
    _isDrawerOpen = !_isDrawerOpen;
    notifyListeners();
  }

  void updateKeyword(String? value) {
    _keyword = value;
    notifyListeners();
  }

  void updateLocation(Location? value) {
    _location = value;
    notifyListeners();
  }

  void updateCategory(OppCategory? value) {
    _category = value;
    notifyListeners();
  }

  void updateVacancies(int? value) {
    _vacancies = value;
    notifyListeners();
  }

  void updateMinPrice(double? value) {
    _minPrice = value;
    notifyListeners();
  }

  void updateMaxPrice(double? value) {
    _maxPrice = value;
    notifyListeners();
  }

  void clear() {
    _keywordController.clear();
    _vacanciesController.clear();
    _minPriceController.clear();
    _maxPriceController.clear();
    _location = null;
    _category = null;
    notifyListeners();
  }

  final Map<String, Future<void> Function(SearchState)> _sortOptions = {
    'Classificação Descendente': (SearchState searchState) =>
        searchState.sortByScore(),
    'Preço Descendente': (SearchState searchState) => searchState.sortByPrice(),
    'Vacancies Descendente': (SearchState searchState) =>
        searchState.sortByVacancies(),
  };

  List<Opportunity>? get opportunitiesList => _opportunitiesList;
  bool get isLoading => _isLoading;
  bool get isTopChecked => _isTopChecked;
  String? get selectedSort => _selectedSort;
  bool get isDrawerOpen => _isDrawerOpen;
  String? get keyword => _keyword;
  Location? get location => _location;
  OppCategory? get category => _category;
  int? get vacancies => _vacancies;
  double? get minPrice => _minPrice;
  double? get maxPrice => _maxPrice;
  TextEditingController get keywordController => _keywordController;
  TextEditingController get vacanciesController => _vacanciesController;
  TextEditingController get minPriceController => _minPriceController;
  TextEditingController get maxPriceController => _maxPriceController;
  Map<String, Future<void> Function(SearchState)> get sortOptions =>
      _sortOptions;

  void setSortOption(String? sortOption) {
    _selectedSort = sortOption;
    notifyListeners();
  }

  void setSelectedSort(String? sortOption) {
    _selectedSort = sortOption;
    notifyListeners();
  }

  Future<void> toggleTopChecked() async {
    _isTopChecked = !_isTopChecked;
    await search();
    notifyListeners();
  }

  Future<void> initializeOpportunitiesList() async {
    _isLoading = true;
    notifyListeners();

    _opportunitiesList =
        await _opportunityApiHandler.getAllOpportunities() ?? [];

    if (_isTopChecked) {
      _opportunitiesList = _opportunitiesList.where((opp) {
        return opp.isImpulsed;
      }).toList();
    }

    _isLoading = false;
    notifyListeners();
  }

  Future<void> search() async {
    _isLoading = true;
    notifyListeners();

    _opportunitiesList = await _opportunityApiHandler.searchOpportunities(
        _keyword, _vacancies, _minPrice, _maxPrice, _category, _location);

    if (_keyword == null &&
        _vacancies == null &&
        _minPrice == null &&
        _maxPrice == null &&
        _category == null &&
        _location == null) {
      _opportunitiesList =
          await _opportunityApiHandler.getAllOpportunities() ?? [];
    }

    if (_isTopChecked) {
      _opportunitiesList = _opportunitiesList.where((opp) {
        return opp.isImpulsed;
      }).toList();
    }

    _isLoading = false;
    notifyListeners();
    return;
  }

  Future<void> sortByScore() async {
    _isLoading = true;
    notifyListeners(); // Notify UI that sorting is starting

    // Sort the opportunities list by score in descending order
    _opportunitiesList.sort((a, b) => b.reviewScore.compareTo(a.reviewScore));

    _isLoading = false;
    notifyListeners(); // Notify UI that sorting is complete
  }

  Future<void> sortByPrice() async {
    _isLoading = true;
    notifyListeners(); // Notify UI that sorting is starting

    // Sort the opportunities list by score in descending order
    _opportunitiesList.sort((a, b) => b.price.compareTo(a.price));

    _isLoading = false;
    notifyListeners(); // Notify UI that sorting is complete
  }

  Future<void> sortByVacancies() async {
    _isLoading = true;
    notifyListeners(); // Notify UI that sorting is starting

    // Sort the opportunities list by price in descending order
    _opportunitiesList.sort((a, b) => b.vacancies.compareTo(a.vacancies));

    _isLoading = false;
    notifyListeners(); // Notify UI that sorting is complete
  }

  @override
  void dispose() {
    _keywordController.dispose();
    _vacanciesController.dispose();
    _minPriceController.dispose();
    _maxPriceController.dispose();
    super.dispose();
  }
}
