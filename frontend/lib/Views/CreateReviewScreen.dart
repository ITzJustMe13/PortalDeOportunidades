import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart';
import 'package:frontend/Models/Reservation.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Services/review_api_handler.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';

class CreateReviewScreen extends StatefulWidget {
  final Reservation reservation;

  const CreateReviewScreen({super.key, required this.reservation});

  @override
  _CreateReviewScreenState createState() => _CreateReviewScreenState();
}
  
class _CreateReviewScreenState extends State<CreateReviewScreen> {

  late TextEditingController descriptionController;
  late TextEditingController ratingController;

  @override
  void initState() {
    descriptionController = TextEditingController();
    ratingController = TextEditingController();
    
    getCreatedReview(widget.reservation);
  }

  @override
  void dispose() {
    descriptionController.dispose();
    ratingController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: CustomAppBar(),
        endDrawer: CustomDrawer(),
        );
  }

  Future<void> getCreatedReview(Reservation reservation) async {
    if (reservation.reservationId != null) {
      try {
        final existingReview = await Provider.of<ReviewApiHandler>(context, listen: false)
            .getReviewById(reservation.reservationId!);

        if (existingReview != null) {
          descriptionController.text = existingReview.description!;
          ratingController.text = existingReview.rating.toString();
        }
      } catch (e) {
        return;
      }
    } else {
      return;
    }
  }

  
}