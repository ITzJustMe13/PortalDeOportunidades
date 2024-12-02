import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/Reservation.dart';
import 'package:frontend/Models/Review.dart';

class ReviewOpportunityReservation {
  final Review review;
  final Opportunity opportunity;
  final Reservation reservation;

  ReviewOpportunityReservation({
    required this.review,
    required this.opportunity,
    required this.reservation,
  });
}