import 'package:flutter/material.dart';
import 'package:frontend/Models/Favorite.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/Reservation.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Services/payment_api_handler.dart';
import 'package:frontend/Services/payment_service.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:url_launcher/url_launcher.dart';

import '../Models/Review.dart';
import '../Models/User.dart';

class DetailsState extends ChangeNotifier {
  final _userApiHandler = UserApiHandler();
  final _opportunityApiHandler = OpportunityApiHandler();
  final _paymentApiHandler = PaymentApiHandler();

  bool _isReviewsLoading = false;
  bool _isAddFavoriteLoading = false;
  bool _isLoading = false;
  String _errorMessage = "";
  double? _priceWithTax;
  List<Review> _reviews = [];
  bool _isOwner = false;
  User? _owner;

  bool get isReviewsLoading => _isReviewsLoading;
  bool get isAddFavoriteLoading => _isAddFavoriteLoading;
  bool get isLoading => _isLoading;
  String get errorMessage => _errorMessage;
  double? get priceWithTax => _priceWithTax;
  List<Review> get reviews => _reviews;
  bool get isOwner => _isOwner;
  User? get owner => _owner;

  DetailsState();

  Future<void> initialize(Opportunity opportunity) async {
    _isLoading = true;
    notifyListeners();
    _owner = await _userApiHandler.getUserByID(opportunity.userId);

    _checkUserOwnership();

    fetchReviews(opportunity);
    _priceWithTax = double.parse(
        (opportunity.price + (opportunity.price * 0.1)).toStringAsFixed(2));
    _isLoading = false;
    notifyListeners();
  }

  Future<void> fetchReviews(Opportunity o) async {
    _isReviewsLoading = true;
    notifyListeners();

    try {
      final List<Review>? fetchedReviews =
          await _opportunityApiHandler.getReviewsByOppId(o.opportunityId);

      _reviews = fetchedReviews ?? [];
      _isReviewsLoading = false;
      notifyListeners();
    } catch (error) {
      _isReviewsLoading = false;
      _reviews = [];
      notifyListeners();
    }
  }

  /// Documentation for addToFavortites
  /// @param: BuildContext context
  /// @param: int oppId Opportunity id
  /// This Function adds an Opportunity to a user's Favorites
  Future<void> addToFavorites(BuildContext context, int oppId) async {
    _isAddFavoriteLoading = true;
    notifyListeners();

    User? user = await _userApiHandler.getStoredUser();

    if (!context.mounted) {
      _isAddFavoriteLoading = false;
      notifyListeners();
      return;
    }

    if (user == null) {
      _isAddFavoriteLoading = false;
      notifyListeners();
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
              'Erro ao adicionar aos seus Favoritos, você precisa estar logado para fazer isso.'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    final favorite = Favorite(userId: user.userId, opportunityId: oppId);
    final Favorite? addedFavorite = await _userApiHandler.addFavorite(favorite);

    _isAddFavoriteLoading = false;
    notifyListeners();

    if (!context.mounted) return;

    if (addedFavorite != null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Oportunidade adicionada aos seus Favoritos!'),
          backgroundColor: Colors.green,
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
              'Erro ao adicionar aos seus Favoritos, a Oportunidade já existe na sua lista ou tente novamente mais tarde.'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _checkUserOwnership() async {
    _isLoading = true;
    notifyListeners();
    try {
      User? loggedInUser = await _userApiHandler.getStoredUser();
      _isOwner = loggedInUser?.userId == _owner?.userId;
      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _isOwner = false;
      notifyListeners();
    }
  }

  Future<void> createTempReservation(BuildContext context, int numberOfPersons,
      Opportunity opportunity) async {
    _isLoading = true;
    notifyListeners();

    User? user = await _userApiHandler.getStoredUser();

    if (!context.mounted) {
      _isLoading = false;
      notifyListeners();
      return;
    }

    if (user == null) {
      _isLoading = false;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
              'Erro ao criar reserva, você precisa estar logado para fazer isso.'),
          backgroundColor: Colors.red,
        ),
      );
      notifyListeners();
      return;
    }

    final reservation = Reservation(
        opportunityId: opportunity.opportunityId,
        userId: user.userId,
        checkInDate: opportunity.date,
        numOfPeople: numberOfPersons,
        reservationDate: DateTime.now(),
        isActive: true,
        fixedPrice: (opportunity.price * 0.1) + opportunity.price);

    await PaymentService().saveReservation(reservation);
    bool isSuccess = await createCheckoutSessionReservation(reservation);

    if (!isSuccess) {
      _isLoading = false;
      notifyListeners();
      _isLoading = false;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Erro ao criar reserva.'),
          backgroundColor: Colors.red,
        ),
      );
      notifyListeners();
      return;
    }

    _errorMessage = "";
    _isLoading = false;
    notifyListeners();
    return;
  }

  Future<bool> createCheckoutSessionReservation(Reservation reservation) async {
    final sessionUrl =
        await _paymentApiHandler.createReservationCheckoutSession(reservation);

    if (sessionUrl == null) {
      return false;
    }

    if (await canLaunch(sessionUrl)) {
      await launch(sessionUrl);
      return true;
    }

    return false;
  }
}
