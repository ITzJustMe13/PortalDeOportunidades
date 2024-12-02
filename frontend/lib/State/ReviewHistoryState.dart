import 'package:flutter/material.dart';
import 'package:frontend/Models/Review.dart';
import 'package:frontend/Models/ReviewOpportunityReservation.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Services/reservation_api_handler.dart';
import 'package:frontend/Services/review_api_handler.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:http/http.dart' as http;

class ReviewHistoryState with ChangeNotifier {
  final _reservationApiHandler = ReservationApiHandler(http.Client());
  final _userApiHandler = UserApiHandler(http.Client());
  final _opportunityApiHandler = OpportunityApiHandler(http.Client());
  final _reviewApiHandler = ReviewApiHandler(http.Client());

  List<ReviewOpportunityReservation> _reviewList = [];
  bool _isLoading = false;
  String _err = "";
  int _currentPage = 1;

  ReviewHistoryState() {
    getReviewList();
  }

  List<ReviewOpportunityReservation> get reviewList => _reviewList;
  bool get isLoading => _isLoading;
  String get error => _err;
  int get currentPage => _currentPage;

  set currentPage(int value) {
    _currentPage = value;
    notifyListeners();
  }

  Future<void> getReviewList() async {
    _isLoading = true;
    notifyListeners();

    final userId = await _userApiHandler.getStoredUserID();

    if (userId == -1) {
      _isLoading = false;
      _err = "Erro ao carregar comentários";
      _reviewList = [];
      notifyListeners();
      return;
    }

    List<Review> reviews = await _reviewApiHandler.getReviewsByUserId(userId);

    if (reviews.isEmpty) {
      _isLoading = false;
      notifyListeners();
      return;
    }

    _reviewList.clear();
    for (var review in reviews) {
      var reservation =
          await _reservationApiHandler.getReservationById(review.reservationId);

      if (reservation != null) {
        var opportunity = await _opportunityApiHandler
            .getOpportunityByID(reservation.opportunityId);

        if (opportunity != null) {
          _reviewList.add(
            ReviewOpportunityReservation(
              review: review,
              opportunity: opportunity,
              reservation: reservation,
            ),
          );
        }
      }
    }

    _isLoading = false;
    notifyListeners();
  }
}
